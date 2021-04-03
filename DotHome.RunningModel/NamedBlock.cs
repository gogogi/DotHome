using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.RunningModel
{
    /// <summary>
    /// A <see cref="Block"/> with a name parameter
    /// </summary>
    public abstract class NamedBlock : Block
    {
        [Parameter(true), Description("Name of the block")]
        public string Name { get; set; } = "Named Block";
    }
}
