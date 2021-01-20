using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Wire : INotifyPropertyChanged
    {
        public Input Input { get; set; }

        public Output Output { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
