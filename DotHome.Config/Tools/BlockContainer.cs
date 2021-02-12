using DotHome.Config.Views;
using DotHome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockContainer
    {
        public List<Block> Blocks { get; } = new List<Block>();

        public List<Wire> Wires { get; } = new List<Wire>();

        public double MinX { get; set; }

        public double MinY { get; set; }

        public double MaxX { get; set; }

        public double MaxY { get; set; }

        public BlockContainer Copy()
        {
            var bv = this.Blocks;
            BlockContainer blockContainer = new BlockContainer() { MinX = MinX, MaxX = MaxX, MinY = MinY, MaxY = MaxY };

            Dictionary<Value, Value> inputsDictionary = new Dictionary<Value, Value>();
            Dictionary<Value, Value> outputsDictionary = new Dictionary<Value, Value>();

            foreach (Block b in Blocks)
            {
                var b2 = (Block)Activator.CreateInstance(b.GetType());
                b2.X = b.X;
                b2.Y = b.Y;
                for(int i = 0; i < b.Inputs.Count; i++)
                {
                    b2.Inputs[i].Disabled = b.Inputs[i].Disabled;
                    inputsDictionary.Add(b.Inputs[i], b2.Inputs[i]);
                }
                for (int i = 0; i < b.Outputs.Count; i++)
                {
                    b2.Outputs[i].Disabled = b.Outputs[i].Disabled;
                    outputsDictionary.Add(b.Outputs[i], b2.Outputs[i]);
                }
                foreach(PropertyInfo propertyInfo in b.GetType().GetProperties().Where(pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Parameter<>)))
                {

                    propertyInfo.SetValue(b2, propertyInfo.GetValue(b));
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
