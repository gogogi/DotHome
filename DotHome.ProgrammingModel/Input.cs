using DotHome.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Input : IInput
    {
        public InputDefinition Definition { get; private set; }

        public bool Disabled { get; set; }

        public Input(InputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }
    }
}
