using DotHome.Definitions;
using DotHome.ProgrammingModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Parameter : INotifyPropertyChanged
    {
        public ParameterDefinition Definition { get; set; }

        //[ParameterValidation]
        public object Value { get; set; }

        [JsonIgnore]
        public bool ValueAsBool { get => (bool)Value; set => Value = value; }
        [JsonIgnore]
        public int ValueAsInt { get => (int)Value; set => Value = value; }
        [JsonIgnore]
        public uint ValueAsUint { get => (uint)Value; set => Value = value; }
        [JsonIgnore]
        public double ValueAsDouble { get => (double)Value; set => Value = value; }
        [JsonIgnore]
        public string ValueAsString { get => (string)Value; set => Value = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Parameter(ParameterDefinition definition)
        {
            Definition = definition;
            Value = definition.DefaultValue;
            //ParameterValidationAttribute parameterValidationAttribute = GetType().GetProperty(nameof(Value)).GetCustomAttribute<ParameterValidationAttribute>();
            //parameterValidationAttribute.ValidationAttributes.AddRange(Definition.ValidationAttributes);
        }
    }
}
