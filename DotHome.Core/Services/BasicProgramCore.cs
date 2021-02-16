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
        private BlocksActivator blocksActivator;
        private IHubContext<DebuggingHub> debuggingHubContext;

        private double[] usageHistory = new double[usageSamples];
        private int usageIndex = 0;
        private int usageCount = 0;
        private EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private List<ABlock> blocks;
        private List<IBlockService> services;
        private CancellationTokenSource cancellationTokenSource;
        private Task runTask;
        private Stopwatch stopwatch = new Stopwatch();
        private int period;
        private bool isDebugging, isPaused;
        private Dictionary<int, Tuple<object[], object[]>> debugValues = new Dictionary<int, Tuple<object[], object[]>>();

        public double AverageCpuUsage => usageHistory.Take(usageCount).Average();

        public double MaxCpuUsage => usageHistory.Take(usageCount).Max();

        public List<AVisualBlock> VisualBlocks { get; private set; }

        public List<User> Users { get; private set; }

        public List<Room> Rooms { get; private set; }

        public List<Category> Categories { get; private set; }

        public BasicProgramCore(ProgrammingModelLoader programmingModelLoader, BlocksActivator blocksActivator, IHubContext<DebuggingHub> debuggingHubContext)
        {
            this.programmingModelLoader = programmingModelLoader;
            this.blocksActivator = blocksActivator;
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
            isPaused = false;
            debugValues.Clear();
            eventWaitHandle.Set();
        }

        private void Start()
        {
            Load();
            cancellationTokenSource = new CancellationTokenSource();
            runTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Restart()
        {
            bool wasDebugging = isDebugging;
            StopDebugging();
            cancellationTokenSource?.Cancel();
            runTask?.Wait();
            Unload();
            Start();
            if (wasDebugging) StartDebugging();
        }

        public void Pause()
        {
            if (isDebugging) isPaused = true;
        }

        public void Continue()
        {
            isPaused = false;
            eventWaitHandle.Set();
        }

        public void Step()
        {
            eventWaitHandle.Set();
        }

        private void Load()
        {
            Project programmingModel = programmingModelLoader.LoadProgrammingModel();
            period = programmingModel.ProgramPeriod;
            blocks = new List<ABlock>();
            VisualBlocks = new List<AVisualBlock>();

            Users = programmingModel.Users.ToList();
            Rooms = programmingModel.Rooms.ToList();
            Categories = programmingModel.Categories.ToList();

            foreach (Page page in programmingModel.Pages)
            {
                Dictionary<Input, AValue> inputsDictionary = new Dictionary<Input, AValue>();
                Dictionary<Output, AValue> outputsDictionary = new Dictionary<Output, AValue>();

                List<Block> sortedBlocks = BlocksSorter.SortTopological(page.Blocks.ToList(), page.Wires.ToList());

                foreach (Block b in sortedBlocks)
                {
                    ABlock block = blocksActivator.CreateBlock(b);
                    block.Id = b.Id;

                    for (int i = 0; i < b.Inputs.Count; i++) inputsDictionary.Add(b.Inputs[i], block.Inputs[i]);
                    for (int i = 0; i < b.Outputs.Count; i++) outputsDictionary.Add(b.Outputs[i], block.Outputs[i]);

                    blocks.Add(block);
                    if (block is AVisualBlock vb) VisualBlocks.Add(vb);
                }
                foreach (Wire wire in page.Wires)
                {
                    var input = inputsDictionary[wire.Input];
                    var output = outputsDictionary[wire.Output];
                    output.AttachValue(input);
                }
                VisualBlocks.Sort((a, b) => a.Name.CompareTo(b.Name));
            }

            services = blocksActivator.BlockServices;
        }

        private void Unload()
        {
            foreach(ABlock block in blocks)
            {
                if (block is IDisposable d) d.Dispose();
            }
            foreach(IBlockService blockService in services)
            {
                if (blockService is IDisposable d) d.Dispose();
            }
            blocks = null;
            services = null;
            VisualBlocks = null;
            Rooms = null;
            Categories = null;
            Users = null;
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

                if (period > stopwatch.Elapsed.TotalMilliseconds) await Task.Delay(TimeSpan.FromMilliseconds(period) - stopwatch.Elapsed);

                if(isPaused)
                {
                    eventWaitHandle.WaitOne();
                }
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
                    for (int i = 0; i < block.Inputs.Count; i++)
                    {
                        if (!Equals(block.Inputs[i].ValAsObject, tuple.Item1[i]))
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