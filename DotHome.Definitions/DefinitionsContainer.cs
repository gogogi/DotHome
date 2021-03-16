using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    public class DefinitionsContainer
    {
        public List<BlocksCategory> BlockCategories { get; } = new List<BlocksCategory>();

        //public Dictionary<Type, Type> CommunicationProviders { get; } = new Dictionary<Type, Type>();

        public BlockDefinition GetBlockDefinitionByFullName(string name)
        {
            foreach (BlocksCategory category in BlockCategories)
            {
                foreach (BlockDefinition blockDefinition in category.BlockDefinitions)
                {
                    if (blockDefinition.Type.FullName == name)
                    {
                        return blockDefinition;
                    }
                }
            }
            return null;
        }
    }
}
