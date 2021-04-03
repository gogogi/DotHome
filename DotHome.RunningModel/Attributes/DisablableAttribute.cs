using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    /// <summary>
    /// Marks <see cref="Input{T}"/> or <see cref="Output{T}"/> property of a <see cref="Block"/> as disablable in Config GUI.
    /// Disabling an I/O can affect also function of the program (for example logical And - disabled input does nothing 
    /// but enabled and unconnected input has value false and causes output to be always false)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisablableAttribute : Attribute
    {
        public bool DefaultDisabled { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultDisabled">Determines if the I/O should be disabled by default</param>
        public DisablableAttribute(bool defaultDisabled)
        {
            DefaultDisabled = defaultDisabled;
        }
    }
}
