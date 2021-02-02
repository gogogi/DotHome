using DotHome.Core.Server.Hubs;
using DotHome.Core.Server.Tools;
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

namespace DotHome.Core.Server.Services
{
    public class BasicProgramCore : IProgramCore
    {
        private ProgrammingModelLoader programmingModelLoader;
        private IHubContext<DebuggingHub> debuggingHubContext;

        private List<IBlockService> services;
        private List<ABlock> blocks;
        private CancellationTokenSource cancellationTokenSource;
        private Task runTask;
        private Stopwatch stopwatch = new Stopwatch();
        private int period;
        private bool isDebugging;
        private Dictionary<int, Tuple<object[], object[]>> debugValues = new Dictionary<int, Tuple<object[], object[]>>();


        public BasicProgramCore(ProgrammingModelLoader modelLoader, IHubContext<DebuggingHub> debuggingHubContext)
        {
            this.programmingModelLoader = modelLoader;
            this.debuggingHubContext = debuggingHubContext;
        }

        public void StartDebugging()
        {
            foreach(ABlock block in blocks)
            {
                debugValues.Add(block.Id, new Tuple<object[], object[]>(Enumerable.Repeat<object>(null, block.Inputs.Count).ToArray(), Enumerable.Repeat<object>(null, block.Inputs.Count).ToArray())); ;
            }
            isDebugging = true;
        }

        public void StopDebugging()
        {
            isDebugging = false;
            debugValues.Clear();
        }

        public void Start()
        {
            Load();
            cancellationTokenSource = new CancellationTokenSource();
            runTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            runTask?.Wait();
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
            while(!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                Loop();
                stopwatch.Stop();
                int elapsed = (int)stopwatch.ElapsedMilliseconds;
                Debug.WriteLine(elapsed);
                if (period > elapsed) await Task.Delay((int)(period - stopwatch.ElapsedMilliseconds));
            }
        }

        private void Init()
        {
            foreach(IBlockService service in services)
            {
                service.Init();
            }
            foreach(ABlock block in blocks)
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
                block.Run();
                foreach(AValue output in block.Outputs)
                {
                    output.Transfer();
                }
                if(isDebugging)
                {
                    var tuple = debugValues[block.Id];
                    for(int i = 0; i < block.Inputs.Count; i++)
                    {
                        if(!Equals(block.Inputs[i].ValAsObject, tuple.Item1[i]))
                        {
                            tuple.Item1[i] = block.Inputs[i].ValAsObject;
                            debuggingHubContext.Clients.All.SendAsync("Input", block.Id, i, tuple.Item1[i]);
                        }
                    }
                    for (int i = 0; i < block.Outputs.Count; i++)
                    {
                        if (!Equals(block.Outputs[i].ValAsObject, tuple.Item2[i]))
                        {
                            tuple.Item2[i] = block.Outputs[i].ValAsObject;
                            debuggingHubContext.Clients.All.SendAsync("Output", block.Id, i, tuple.Item2[i]);
                        }
                    }
                }
            }
        }
    }
}
