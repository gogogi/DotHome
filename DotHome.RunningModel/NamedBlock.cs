using DotHome.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.Model
{
    public abstract class NamedBlock : Block
    {
        [BlockParameter(true), Description("Name of the block")]
        public string Name { get; set; } = "Named Block";
    }
}
