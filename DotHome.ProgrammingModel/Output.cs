using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Output : INotifyPropertyChanged
    {
        public OutputDefinition Definition { get; set; }

        public bool Disabled { get; set; }

        [JsonIgnore]
        public object DebugValue { get; set; }

        public Output(OutputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
