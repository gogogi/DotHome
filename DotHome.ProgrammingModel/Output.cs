using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    /// <summary>
    /// Output of a <see cref="Block"/>, representing <see cref="RunningModel.Output{T}"/> of <see cref="RunningModel.Block"/>, outcoming value from a block.
    /// </summary>
    public class Output : INotifyPropertyChanged
    {
        /// <summary>
        /// Metadata of <see cref="RunningModel.Output{T}"/> this instance represents
        /// </summary>
        public OutputDefinition Definition { get; set; }

        /// <summary>
        /// Determines if the output is disabled (Hiden in Config GUI)
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Current value of corresponding <see cref="RunningModel.Output{T}"/> in Core is transfered in here and used as binding in Config when in debug mode.
        /// </summary>
        [JsonIgnore]
        public object DebugValue { get; set; }

        public Output(OutputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
