using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class ANamedBlock : ABlock
    {
        [BlockParameter(true), Description("Name of the block")]
        public string Name { get; set; } = "Named Block";
    }
}
