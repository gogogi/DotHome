using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Input : IInput, INotifyPropertyChanged
    {
        public InputDefinition Definition { get; private set; }

        public bool Disabled { get; set; }

        public Input(InputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
