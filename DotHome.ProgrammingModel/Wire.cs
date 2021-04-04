using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    /// <summary>
    /// A line connecting <see cref="ProgrammingModel.Input"/> and <see cref="ProgrammingModel.Output"/> of 2 <see cref="Block"/>s inside a <see cref="Page"/>
    /// </summary>
    public class Wire : INotifyPropertyChanged
    {
        public Input Input { get; set; }

        public Output Output { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
