using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public abstract class ABlock : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        //public int Width { get; set; }

        //public int Height { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual IInput[] GetInputs() => new IInput[0];

        public virtual IOutput[] GetOutputs() => new IOutput[0];
    }
}