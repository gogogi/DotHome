using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Wire : INotifyPropertyChanged
    {
        public IInput Input { get; set; }

        public IOutput Output { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
