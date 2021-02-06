using DotHome.Core.Hubs;
using DotHome.Core.Tools;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public class BasicProgramCore : IProgramCore
    {
        private const int usageSamples = 100;

        private ProgrammingModelLoader programmingModelLoader;
        private IHubContext<DebuggingHub> debuggingHubContext;
        private double[] usageHistory = new double[usageSamples];
        private int usageIndex = 0;
        private int usageCount = 0;

        private List<IBlockService> services;
        private List<ABlock> blocks;
        private CancellationTokenSource cancellationTokenSource;
        private Task runTask;
        private Stopwatch stopwatch = new Stopwatch();
        private int period;
        private bool isDebugging;
        private Dictionary<int, Tuple<object[], object[]>> debugValues = new Dictionary<int, Tuple<object[], object[]>>();

        public double AverageCpuUsage => usageHistory.Take(usageCount).Average();

        public double MaxCpuUsage => usageHistory.Take(usageCount).Max();

        public BasicProgramCore(ProgrammingModelLoader programmingModelLoader, IHubContext<DebuggingHub> debuggingHubContext)
        {
            this.programmingModelLoader = programmingModelLoader;
            this.debuggingHubContext = debuggingHubContext;
            Start();
        }

        public void StartDebugging()
        {
            foreach (ABlock block in blocks)
            {
                debugValues.Add(block.Id, new Tuple<object[], object[]>(Enumerable.Repeat<object>(null, block.Inputs.Count).ToArray(), Enumerable.Repeat<object>(null, block.Outputs.Count).ToArray())); ;
            }
            isDebugging = true;
        }

        public void StopDebugging()
        {
            isDebugging = false;
            debugValues.Clear();
        }

        private void Start()
        {
            Load();
            cancellationTokenSource = new CancellationTokenSource();
            runTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        private void Load()
        {
            Project programmingModel = programmingModelLoader.LoadProgrammingModel();
            period = programmingModel.ProgramPeriod;
            blocks = new List<ABlock>();

            services = new List<IBlockService>();

            foreach (Page page in programmingModel.Pages)
            {
                Dictionary<Input, AValue> inputsDictionary = new Dictionary<Input, AValue>();
                Dictionary<Output, AValue> outputsDictionary = new Dictionary<Output, AValue>();
                Dictionary<AValue, ABlock> inputsBlocksDictionary = new Dictionary<AValue, ABlock>();

                List<Block> sortedBlocks = BlocksSorter.SortTopological(page.Blocks.ToList(), page.Wires.ToList());

                foreach (Block b in sortedBlocks)
                {
                    Type type = b.Definition.Type;
                    ConstructorInfo[] constructors = type.GetConstructors();

                    ConstructorInfo constructor = constructors.First(c => c.GetParameters().All(p => p.ParameterType.IsAssignableTo(typeof(IBlockService))));
                    List<IBlockService> parameters = new List<IBlockService>();
                    foreach (ParameterInfo parameterInfo in constructor.GetParameters())
                    {
                        IBlockService parameter = services.SingleOrDefault(s => s.GetType().IsAssignableTo(parameterInfo.ParameterType));
                        if (parameter == null)
                        {
                            parameter = (IBlockService)Activator.CreateInstance(parameterInfo.ParameterType);
                            services.Add(parameter);
                        }
                        parameters.Add(parameter);
                    }

                    ABlock block = (ABlock)Activator.CreateInstance(type, parameters.Cast<object>().ToArray());
                    block.Id = b.Id;

                    foreach (Input i in b.Inputs)
                    {
                        PropertyInfo pi = type.GetProperty(i.Definition.Name);
                        AValue input = (AValue)Activator.CreateInstance(pi.PropertyType);
                        input.Disabled = i.Disabled;
                        pi.SetValue(block, input);
                        inputsDictionary.Add(i, input);
                        block.Inputs.Add(input);

                        inputsBlocksDictionary.Add(input, block);
                    }

                    foreach (Output o in b.Outputs)
                    {
                        PropertyInfo po = type.GetProperty(o.Definition.Name);
                        AValue output = (AValue)Activator.CreateInstance(po.PropertyType);
                        output.Disabled = o.Disabled;
                        po.SetValue(block, output);
                        outputsDictionary.Add(o, output);
                        block.Outputs.Add(output);
                    }

                    foreach (Parameter p in b.Parameters)
                    {
                        PropertyInfo po = type.GetProperty(p.Definition.Name);
                        po.SetValue(block, p.Value);
                    }
                    blocks.Add(block);
                }
                foreach (Wire wire in page.Wires)
                {
                    var input = inputsDictionary[wire.Input];
                    var output = outputsDictionary[wire.Output];
                    output.AttachValue(input);
                }
            }
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            Init();
            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                Loop();
                stopwatch.Stop();

                double usage = stopwatch.Elapsed.TotalMilliseconds / period;
                usageHistory[usageIndex] = usage;
                usageIndex = (usageIndex + 1) % usageSamples;
                if (usageCount < usageSamples) usageCount++;

                //Debug.WriteLine("Elapsed " + stopwatch.Elapsed);
                //Debug.WriteLine(AverageCpuUsage);
                //Debug.WriteLine(MaxCpuUsage);
                if (period > stopwatch.Elapsed.TotalMilliseconds) await Task.Delay(TimeSpan.FromMilliseconds(period) - stopwatch.Elapsed);
            }
        }

        private void Init()
        {
            foreach (IBlockService service in services)
            {
                service.Init();
            }
            foreach (ABlock block in blocks)
            {
                block.Init();
            }
        }

        private void Loop()
        {
            foreach (IBlockService service in services)
            {
                service.Run();
            }
            foreach (ABlock block in blocks)
            {
                //Debug.WriteLine("Running " + block.Id);
                try
                {
                    block.ClearDebugString();
                    block.Run();
                    if (isDebugging)
                    {
                        debuggingHubContext.Clients.All.SendAsync("BlockException", block.Id, null);
                        debuggingHubContext.Clients.All.SendAsync("Block", block.Id, block.DebugString);
                    }
                }
                catch (Exception e)
                {
                    if (isDebugging)
                    {
                        debuggingHubContext.Clients.All.SendAsync("BlockException", block.Id, e);
                    }
                }

                foreach (AValue output in block.Outputs)
                {
                    output.Transfer();
                }

                if (isDebugging)
                {
                    var tuple = debugValues[block.Id];
                    //Debug.WriteLine("A");
                    for (int i = 0; i < block.Inputs.Count; i++)
                    {
                        //  Debug.WriteLine("B");
                        if (!Equals(block.Inputs[i].ValAsObject, tuple.Item1[i]))
                        {
                            //    Debug.WriteLine("C");
                            tuple.Item1[i] = block.Inputs[i].ValAsObject;
                            //  Debug.WriteLine("D");
                            debuggingHubContext.Clients.All.SendAsync("Input", block.Id, i, tuple.Item1[i]);
                        }
                        //Debug.WriteLine("E");
                    }
                    //Debug.WriteLine("F");
                    for (int i = 0; i < block.Outputs.Count; i++)
                    {
                        //  Debug.WriteLine("G");
                        if (!Equals(block.Outputs[i].ValAsObject, tuple.Item2[i]))
                        {
                            //    Debug.WriteLine("H");
                            tuple.Item2[i] = block.Outputs[i].ValAsObject;
                            //  Debug.WriteLine("I");
                            debuggingHubContext.Clients.All.SendAsync("Output", block.Id, i, tuple.Item2[i]);
                        }
                    }
                    //Debug.WriteLine("J");
                }
            }
        }

        public void Restart()
        {
            bool wasDebugging = isDebugging;
            StopDebugging();
            cancellationTokenSource?.Cancel();
            runTask?.Wait();
            Start();
            if (wasDebugging) StartDebugging();
        }
    }
}