using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    public class ParameterDefinition : ADefinition
    {
        public object DefaultValue { get; set; }
        public Type Type { get; set; }
    }
}
