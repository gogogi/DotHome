using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Const : ABlock, IOutput
    {
        public override IOutput[] GetOutputs() => new IOutput[] { this };

        public Type Type { get; set; }

        public object Value { get; set; }
    }
}
