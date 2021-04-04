using DotHome.Definitions.Tools;
using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Definitions
{
    /// <summary>
    /// When class derived from <see cref="RunningModel.Block"/> is generic, it means that <see cref="BlockDefinition"/> is created for each type in <see cref="RunningModelTools.SupportedTypes"/> and a programming model block can be created for any of them.
    /// But we want to show this block only once in block library of Config GUI and so this definitions are agregated into a single <see cref="GenericBlockDefinition"/>
    /// </summary>
    public class GenericBlockDefinition : BlockDefinition
    {
        private IEnumerable<BlockDefinition> particularDefinitions;

        /// <summary>
        /// Creates a <see cref="GenericBlockDefinition"/> representing the <paramref name="definitions"/> group
        /// </summary>
        /// <param name="type">The genric type for which we are aggregatng the <see cref="BlockDefinition"/>s</param>
        /// <param name="definitions">The group we want to aggregate</param>
        /// <returns></returns>
        public static GenericBlockDefinition Create(Type type, IEnumerable<BlockDefinition> definitions)
        {
            BlockDefinition helperBlockDefinition = definitions.First();
            GenericBlockDefinition genericBlockDefinition = new GenericBlockDefinition()
            {
                Color = helperBlockDefinition.Color,
                Description = helperBlockDefinition.Description,
                Name = helperBlockDefinition.Name.Remove(helperBlockDefinition.Name.IndexOf('<')) + "<>",
                Type = type,
                particularDefinitions = definitions
            };
            foreach(InputDefinition inputDefinition in helperBlockDefinition.Inputs)
            {
                genericBlockDefinition.Inputs.Add(inputDefinition);
            }
            foreach (OutputDefinition outputDefinition in helperBlockDefinition.Outputs)
            {
                genericBlockDefinition.Outputs.Add(outputDefinition);
            }
            foreach (ParameterDefinition parameterDefinition in helperBlockDefinition.Parameters)
            {
                genericBlockDefinition.Parameters.Add(parameterDefinition);
            }
            return genericBlockDefinition;
        }

        private GenericBlockDefinition() { }

        /// <summary>
        /// Gets the particular <see cref="BlockDefinition"/> with <paramref name="type"/> as the single generic type argument
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BlockDefinition GetParticularBlockDefinition(Type type)
        {
            foreach(BlockDefinition blockDefinition in particularDefinitions)
            {
                if(blockDefinition.Type.GetGenericArguments().Single() == type)
                {
                    return blockDefinition;
                }
            }
            return null;
        }
    }
}
