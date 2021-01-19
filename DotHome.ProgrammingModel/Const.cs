using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Const : ABlock, IOutput, INotifyPropertyChanged
    {
        public override IOutput[] GetOutputs() => new IOutput[] { this };

        public Type Type { get; set; }

        public object Value { get; set; }
    }
}
