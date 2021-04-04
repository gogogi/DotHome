using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Container contains metadata of types derived from <see cref="RunningModel.Block"/> from all currently used block libraries (like StandardBlocks)
    /// </summary>
    public class DefinitionsContainer
    {
        public List<BlocksCategory> BlockCategories { get; } = new List<BlocksCategory>();

        /// <summary>
        /// Gets <see cref="BlockDefinition"/> with given type name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
