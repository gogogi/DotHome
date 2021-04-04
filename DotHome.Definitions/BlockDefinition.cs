using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Object representing metadata of type derived from <see cref="RunningModel.Block"/>
    /// </summary>
    public class BlockDefinition : ADefinition
    {
        public List<InputDefinition> Inputs { get; } = new List<InputDefinition>();
        public List<OutputDefinition> Outputs { get; } = new List<OutputDefinition>();
        public List<ParameterDefinition> Parameters { get; } = new List<ParameterDefinition>();

        /// <summary>
        /// Color to be used in Config GUI, given by <see cref="RunningModel.Attributes.ColorAttribute"/>
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Type this metadata object is representing
        /// </summary>
        public Type Type { get; set; }
    }
}
