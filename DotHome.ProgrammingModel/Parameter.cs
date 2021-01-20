using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Parameter : INotifyPropertyChanged
    {
        public ParameterDefinition Definition { get; set; }

        public object Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Parameter(ParameterDefinition definition)
        {
            Definition = definition;
            Value = definition.DefaultValue;
        }
    }
}
