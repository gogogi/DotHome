using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class RefSink : ABlock, IInput, INotifyPropertyChanged
    {
        public override IInput[] GetInputs() => new IInput[] { this };

        public string Reference { get; set; }
    }
}
