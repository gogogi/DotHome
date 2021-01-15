using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Block : ABlock
    {
        public BlockDefinition Definition { get; set; }

        public List<Input> Inputs { get; } = new List<Input>();

        public List<Output> Outputs { get; } = new List<Output>();

        public override IInput[] GetInputs() => (IInput[])Inputs.ToArray();

        public override IOutput[] GetOutputs() => (IOutput[])Outputs.ToArray();

        public Block(BlockDefinition definition)
        {
            Definition = definition;
            foreach (var input in Definition.Inputs)
            {
                Inputs.Add(new Input(input));
            }

            foreach (var output in Definition.Outputs)
            {
                Outputs.Add(new Output(output));
            }
        }
    }
}
