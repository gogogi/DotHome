using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Base class for all definitions. A definition represents metadata of objects from namespace <see cref="RunningModel"/>
    /// </summary>
    public abstract class ADefinition
    {
        /// <summary>
        /// Name of programming model object this metadata object represents
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of programming model object this metadata object represents (given by <see cref="System.ComponentModel.DescriptionAttribute"/>)
        /// </summary>
        public string Description { get; set; }
    }
}
