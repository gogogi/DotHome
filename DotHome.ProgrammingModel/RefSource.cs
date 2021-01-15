using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class RefSource : ABlock, IOutput
    {
        public override IOutput[] GetOutputs() => new IOutput[] { this };

        public string Reference { get; set; }
    }
}
