using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockContainer
    {
        public List<ABlock> Blocks { get; } = new List<ABlock>();

        public List<Wire> Wires { get; } = new List<Wire>();
    }
}
