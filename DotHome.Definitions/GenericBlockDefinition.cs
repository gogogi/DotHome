using DotHome.Definitions.Tools;
using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Definitions
{
    public class GenericBlockDefinition : BlockDefinition
    {
        private IEnumerable<BlockDefinition> particularDefinitions;

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
