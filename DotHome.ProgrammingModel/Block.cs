using DotHome.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.ProgrammingModel
{
    /// <summary>
    /// Programming model program organization unit representing <see cref="RunningModel.Block"/>
    /// </summary>
    public class Block : INotifyPropertyChanged, IProgrammingModelObject
    {
        /// <summary>
        /// Id which is responsible for attachig programming model block to running model block during debug (<seealso cref="RunningModel.Block.Id"/>)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Block x coordinate on <see cref="Page"/> in pixels
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Block y coordinate on <see cref="Page"/> in pixels
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Metadata of <see cref="RunningModel.Block"/> this instance represents
        /// </summary>
        public BlockDefinition Definition { get; set; }

        /// <summary>
        /// Determines if the block is currently selected in Config GUI. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public bool Selected { get; set; }

        /// <summary>
        /// When in debug mode, exceptions that occur in <see cref="RunningModel.Block.Run"/> are transfered in here and displayed in Config GUI. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public Exception Exception { get; set; }

        /// <summary>
        /// When in debug mode, messages written in <see cref="RunningModel.Block.Run"/> by <see cref="RunningModel.Block.DebugWrite(object)"/> and similar are transfered in here and displayed in Config GUI. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public string DebugString { get; set; }

        /// <summary>
        /// <see cref="Input"/>s representing <see cref="RunningModel.Block.Inputs"/>
        /// </summary>
        public ObservableCollection<Input> Inputs { get; } = new ObservableCollection<Input>();

        /// <summary>
        /// <see cref="Output"/>s representing <see cref="RunningModel.Block.Outputs"/>
        /// </summary>
        public ObservableCollection<Output> Outputs { get; } = new ObservableCollection<Output>();

        /// <summary>
        /// <see cref="Parameter"/> representing properties of <see cref="RunningModel.Block"/> marked by <see cref="RunningModel.Attributes.ParameterAttribute"/>
        /// </summary>
        public ObservableCollection<Parameter> Parameters { get; } = new ObservableCollection<Parameter>();

        public Block(BlockDefinition definition)
        {
            Definition = definition;
            foreach (var input in Definition.Inputs) // Create inputs based on input definitions
            {
                Inputs.Add(new Input(input));
            }

            foreach (var output in Definition.Outputs) // Create outputs based on output definitions
            {
                Outputs.Add(new Output(output));
            }

            foreach(var parameter in Definition.Parameters) // Create parameters based on parameter definitions
            {
                Parameters.Add(new Parameter(parameter));
            }
        }

        public Block() { } // There must be an empty constructor for Json deserialization to work properly

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
