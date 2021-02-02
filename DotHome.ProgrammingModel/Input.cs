using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Input : INotifyPropertyChanged
    {
        public InputDefinition Definition { get; set; }

        public bool Disabled { get; set; }

        [JsonIgnore]
        public object DebugValue { get; set; }

        public Input(InputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
