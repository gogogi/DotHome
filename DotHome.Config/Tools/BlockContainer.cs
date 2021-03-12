using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockContainer
    {
        public List<ProgrammingModel.Block> Blocks { get; } = new List<ProgrammingModel.Block>();

        public List<Wire> Wires { get; } = new List<Wire>();

        public double MinX { get; set; }

        public double MinY { get; set; }

        public double MaxX { get; set; }

        public double MaxY { get; set; }

        public BlockContainer Copy()
        {
            BlockContainer blockContainer = new BlockContainer() { MinX = MinX, MaxX = MaxX, MinY = MinY, MaxY = MaxY };

            Dictionary<Input, Input> inputsDictionary = new Dictionary<Input, Input>();
            Dictionary<Output, Output> outputsDictionary = new Dictionary<Output, Output>();

            foreach (var b in Blocks)
            {
                var b2 = new ProgrammingModel.Block(b.Definition) { X = b.X, Y = b.Y };
                for (int i = 0; i < b.Inputs.Count; i++)
                {
                    b2.Inputs[i].Disabled = b.Inputs[i].Disabled;
                    inputsDictionary.Add(b.Inputs[i], b2.Inputs[i]);
                }
                for (int i = 0; i < b.Outputs.Count; i++)
                {
                    b2.Outputs[i].Disabled = b.Outputs[i].Disabled;
                    outputsDictionary.Add(b.Outputs[i], b2.Outputs[i]);
                }
                for (int i = 0; i < b.Parameters.Count; i++)
                {
                    {
                        b2.Parameters[i].Value = b.Parameters[i].Value;
                    }
                }
                blockContainer.Blocks.Add(b2);
            }

            foreach(var w in Wires)
            {
                blockContainer.Wires.Add(new Wire() { Input = inputsDictionary[w.Input], Output = outputsDictionary[w.Output] });
            }

            return blockContainer;
        }
    }
}
