using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    public class InputDefinition : ADefinition
    {
        public bool Disablable { get; set; }
        public bool DefaultDisabled { get; set; }
        public Type Type { get; set; }
    }
}
