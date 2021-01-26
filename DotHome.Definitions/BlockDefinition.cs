using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DotHome.Definitions
{
    public class BlockDefinition : ADefinition
    {
        public bool IsVisual { get; set; }
        public List<InputDefinition> Inputs { get; } = new List<InputDefinition>();
        public List<OutputDefinition> Outputs { get; } = new List<OutputDefinition>();
        public List<ParameterDefinition> Parameters { get; } = new List<ParameterDefinition>();
        public Color Color { get; set; }

        public Type Type { get; set; }
    }
}
