using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.ProgrammingModel.Tools
{
    public class ParameterValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Parameter parameter = (Parameter)validationContext.ObjectInstance;
            foreach(ValidationAttribute validationAttribute in parameter.Definition.ValidationAttributes)
            {
                var res = validationAttribute.GetValidationResult(value, validationContext);
                if (res != ValidationResult.Success) return res;
            }
            return ValidationResult.Success;
        }
    }
}
