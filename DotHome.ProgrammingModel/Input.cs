using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    /// <summary>
    /// Input of a <see cref="Block"/>, representing <see cref="RunningModel.Input{T}"/> of <see cref="RunningModel.Block"/>, incoming value to a block.
    /// </summary>
    public class Input : INotifyPropertyChanged
    {
        /// <summary>
        /// Metadata of <see cref="RunningModel.Input{T}"/> this instance represents
        /// </summary>
        public InputDefinition Definition { get; set; }

        /// <summary>
        /// Determines if the input is disabled (Hiden in Config GUI)
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Current value of corresponding <see cref="RunningModel.Input{T}"/> in Core is transfered in here and used as binding in Config when in debug mode.
        /// </summary>
        [JsonIgnore]
        public object DebugValue { get; set; }

        public Input(InputDefinition definition)
        {
            Definition = definition;
            Disabled = definition.DefaultDisabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
