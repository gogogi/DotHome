using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DotHome.Definitions
{
    public class ParameterDefinition : ADefinition
    {
        public object DefaultValue { get; set; }

        public Type Type { get; set; }

        public bool ShowInBlock { get; set; } = true;

        public PropertyInfo PropertyInfo { get; set; }

        public List<ValidationAttribute> ValidationAttributes { get; } = new List<ValidationAttribute>();
    }
}
