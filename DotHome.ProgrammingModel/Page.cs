using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    [Serializable]
    public class Page
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<ABlock> Blocks { get; } = new List<ABlock>();

        public List<Wire> Wires { get; } = new List<Wire>();
    }
}
