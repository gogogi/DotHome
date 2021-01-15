using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Output : IOutput
    {
        public OutputDefinition Definition { get; set; }

        public bool Disabled { get; set; }

        public Output(OutputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }
    }
}
