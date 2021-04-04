using DotHome.Definitions.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotHome.Definitions
{
    /// <summary>
    /// Category of <see cref="BlockDefinition"/>s. Category is determined by <see cref="System.ComponentModel.CategoryAttribute"/> applied on type derived from <see cref="RunningModel.Block"/>
    /// </summary>
    public class BlocksCategory
    {
        /// <summary>
        /// Name of category given by <see cref="System.ComponentModel.CategoryAttribute"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Blocks in this category
        /// </summary>
        public List<BlockDefinition> BlockDefinitions { get; } = new List<BlockDefinition>();

        /// <summary>
        /// <see cref="BlockDefinitions"/> for binding purpose. <see cref="BlockDefinition"/>s that represent a generic type are aggregated into <see cref="GenericBlockDefinition"/>s
        /// </summary>
        public IEnumerable<BlockDefinition> AgregatedBlockDefinitions =>
            BlockDefinitions.GroupBy(b => b.Type.IsGenericType ? b.Type.GetGenericTypeDefinition() : b.Type, (k, bb) => k.IsGenericType ? GenericBlockDefinition.Create(k, bb) : bb.Single());
    }
}
