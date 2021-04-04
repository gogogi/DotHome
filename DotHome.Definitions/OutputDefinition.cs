using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Object representing metadata of <see cref="RunningModel.Output{T}"/> properties of <see cref="RunningModel.Block"/>
    /// </summary>
    public class OutputDefinition : ADefinition
    {
        /// <summary>
        /// Determines if the output can be disabled in Config GUI. Is true if the <see cref="RunningModel.Output{T}"/> property is marked with <see cref="RunningModel.Attributes.DisablableAttribute"/>
        /// </summary>
        public bool Disablable { get; set; }

        /// <summary>
        /// Determines if the output is disabled by default in Config GUI. Its value is determined by <see cref="RunningModel.Attributes.DisablableAttribute.DefaultDisabled"/>. If <see cref="Disablable"/> is false, this value must be false as well.
        /// </summary>
        public bool DefaultDisabled { get; set; }

        /// <summary>
        /// The generic type parameter of the represented <see cref="RunningModel.Output{T}"/> property
        /// </summary>
        public Type Type { get; set; }
    }
}
