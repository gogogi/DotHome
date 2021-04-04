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
    /// <summary>
    /// A parameter of <see cref="Block"/>. It represents property of <see cref="RunningModel.Block"/> marked by <see cref="RunningModel.Attributes.ParameterAttribute"/>
    /// </summary>
    public class Parameter : INotifyPropertyChanged
    {
        /// <summary>
        /// Metadata of <see cref="RunningModel.Block"/> property this instance represents
        /// </summary>
        public ParameterDefinition Definition { get; set; }
        
        /// <summary>
        /// Value of the parameter set in Config GUI. The corresponding property of <see cref="RunningModel.Block"/> is initialized with this value in Core.
        /// </summary>
        public object Value { get; set; }

        //Following properties are only helpers for binding in Configu GUI
        [JsonIgnore]
        public bool ValueAsBool { get => (bool)Value; set => Value = value; }
        [JsonIgnore]
        public int ValueAsInt { get => (int)Value; set => Value = value; }
        [JsonIgnore]
        public uint ValueAsUint { get => (uint)Value; set => Value = value; }
        [JsonIgnore]
        public double ValueAsDouble { get => (double)Value; set => Value = value; }
        [JsonIgnore]
        public string ValueAsString { get => Value as string; set => Value = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Parameter(ParameterDefinition definition)
        {
            Definition = definition;
            Value = definition.DefaultValue;
        }
    }
}
