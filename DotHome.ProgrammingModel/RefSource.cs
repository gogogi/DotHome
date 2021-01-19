using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class RefSource : ABlock, IOutput, INotifyPropertyChanged
    {
        public override IOutput[] GetOutputs() => new IOutput[] { this };

        public string Reference { get; set; }
    }
}
