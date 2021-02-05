using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Block : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Selected { get; set; }

        public BlockDefinition Definition { get; set; }

        [JsonIgnore]
        public Exception Exception { get; set; }

        [JsonIgnore]
        public string DebugString { get; set; }

        public ObservableCollection<Input> Inputs { get; } = new ObservableCollection<Input>();

        public ObservableCollection<Output> Outputs { get; } = new ObservableCollection<Output>();

        public ObservableCollection<Parameter> Parameters { get; } = new ObservableCollection<Parameter>();

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

            foreach(var parameter in Definition.Parameters)
            {
                Parameters.Add(new Parameter(parameter));
            }
        }

        public Block() { }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
