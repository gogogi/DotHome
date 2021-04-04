using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Object representing metadata of parameter properties of <see cref="RunningModel.Block"/> (properties marked by <see cref="RunningModel.Attributes.ParameterAttribute"/>)
    /// </summary>
    public class ParameterDefinition : ADefinition
    {
        /// <summary>
        /// Value that is assigned to the property by default
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Type of the property
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Determines if the parameter should show inside the block in Config GUI or only in properties window. Is set according to <see cref="RunningModel.Attributes.ParameterAttribute.ShowInBlock"/>
        /// </summary>
        public bool ShowInBlock { get; set; } = true;

        /// <summary>
        /// <see cref="PropertyInfo"/> of the parameter property
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Validation attributes represent rules for supported values of parameters.
        /// </summary>
        public List<ValidationAttribute> ValidationAttributes { get; } = new List<ValidationAttribute>();
    }
}
