using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    public class DisablableAttribute : Attribute
    {
        public bool DefaultDisabled { get; }

        public DisablableAttribute(bool defaultDisabled)
        {
            DefaultDisabled = defaultDisabled;
        }
    }
}
