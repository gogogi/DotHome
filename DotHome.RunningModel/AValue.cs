using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class AValue
    {
        public abstract Type Type { get; }

        public bool Disabled { get; set; }
    }
}
