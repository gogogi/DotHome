using DotHome.Definitions;
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

        public Output(OutputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
