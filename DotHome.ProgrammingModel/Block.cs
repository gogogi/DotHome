using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Block : ABlock, INotifyPropertyChanged
    {
        public BlockDefinition Definition { get; set; }

        public ObservableCollection<Input> Inputs { get; } = new ObservableCollection<Input>();

        public ObservableCollection<Output> Outputs { get; } = new ObservableCollection<Output>();

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
