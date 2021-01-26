using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.Core.Server.Services
{
    public class BasicProgramCore : IProgramCore
    {
        private ProgrammingModelLoader programmingModelLoader;
        private List<ABlock> blocks;
        private CancellationTokenSource cancellationTokenSource;
        private Task runTask;

        public BasicProgramCore(ProgrammingModelLoader modelLoader)
        {
            this.programmingModelLoader = modelLoader;
            Start();
        }

        public void Reload()
        {
            Stop();
            Start();
        }

        private void Start()
        {
            Load();
            cancellationTokenSource = new CancellationTokenSource();
            runTask = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        private void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private void Load()
        {
            var programmingModel = programmingModelLoader.LoadProgrammingModel();
            blocks = new List<ABlock>();
            foreach (Page page in programmingModel.Pages)
            {
                Dictionary<Input, AValue> inputsDictionary = new Dictionary<Input, AValue>();
                Dictionary<Output, AValue> outputsDictionary = new Dictionary<Output, AValue>();
                foreach (Block b in page.Blocks)
                {
                    Type type = b.Definition.Type;
                    ABlock block = (ABlock)Activator.CreateInstance(type);

                    foreach (Input i in b.Inputs)
                    {
                        PropertyInfo pi = type.GetProperty(i.Definition.Name);
                        AValue input = (AValue)Activator.CreateInstance(pi.PropertyType);
                        input.Disabled = i.Disabled;
                        pi.SetValue(block, input);
                        inputsDictionary.Add(i, input);
                        block.Inputs.Add(input);
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


                    if (output is Output<bool> o_b)
                    {
                        if (input is Input<bool> i_b) o_b.TransferEvent += () => i_b.Val = o_b.Val;
                        else if (input is Input<int> i_i) o_b.TransferEvent += () => i_i.Val = o_b.Val ? 1 : 0;
                        else if (input is Input<float> i_f) o_b.TransferEvent += () => i_f.Val = o_b.Val ? 1 : 0;
                        else if (input is Input<string> i_s) o_b.TransferEvent += () => i_s.Val = o_b.Val.ToString();
                    }
                    else if (output is Output<int> o_i)
                    {
                        if (input is Input<bool> i_b) o_i.TransferEvent += () => i_b.Val = o_i.Val != 0;
                        else if (input is Input<int> i_i) o_i.TransferEvent += () => i_i.Val = o_i.Val;
                        else if (input is Input<float> i_f) o_i.TransferEvent += () => i_f.Val = o_i.Val;
                        else if (input is Input<string> i_s) o_i.TransferEvent += () => i_s.Val = o_i.Val.ToString();
                    }
                    else if (output is Output<float> o_f)
                    {
                        if (input is Input<bool> i_b) o_f.TransferEvent += () => i_b.Val = Math.Abs(o_f.Val) > 1e-5;
                        else if (input is Input<int> i_i) o_f.TransferEvent += () => i_i.Val = (int)o_f.Val;
                        else if (input is Input<float> i_f) o_f.TransferEvent += () => i_f.Val = o_f.Val;
                        else if (input is Input<string> i_s) o_f.TransferEvent += () => i_s.Val = o_f.Val.ToString();
                    }
                    else if (output is Output<string> o_s)
                    {
                        if (input is Input<bool> i_b) o_s.TransferEvent += () => { if (bool.TryParse(o_s.Val, out bool b)) i_b.Val = b; else i_b.Val = false; };
                        else if (input is Input<int> i_i) o_s.TransferEvent += () => { if (int.TryParse(o_s.Val, out int i)) i_i.Val = i; else i_i.Val = 0; };
                        else if (input is Input<float> i_f) o_s.TransferEvent += () => { if (float.TryParse(o_s.Val, out float f)) i_f.Val = f; else i_f.Val = 0; };
                        else if (input is Input<string> i_s) o_s.TransferEvent += () => i_s.Val = o_s.Val;
                    }
                }
            }
        }

        private void Run(CancellationToken cancellationToken)
        {
            Init();
            while(!cancellationToken.IsCancellationRequested)
            {
                Loop();
            }
        }

        private void Init()
        {
            foreach(ABlock block in blocks)
            {
                block.Init();
            }
        }

        private void Loop()
        {
            foreach (ABlock block in blocks)
            {
                block.Run();
                foreach(AValue output in block.Outputs)
                {
                    output.Transfer();
                }
            }
        }
    }
}
