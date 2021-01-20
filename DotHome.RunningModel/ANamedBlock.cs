using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class ANamedBlock : ABlock
    {
        [Parameter(true)]
        public string Name { get; set; } = "Named Block";
    }
}
