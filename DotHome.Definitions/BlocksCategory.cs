using DotHome.Definitions.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotHome.Definitions
{
    public class BlocksCategory
    {
        public string Name { get; set; }
        public List<BlockDefinition> BlockDefinitions { get; } = new List<BlockDefinition>();

        public IEnumerable<BlockDefinition> AgregatedBlockDefinitions =>
            BlockDefinitions.GroupBy(b => b.Type.IsGenericType ? b.Type.GetGenericTypeDefinition() : b.Type, (k, bb) => k.IsGenericType ? GenericBlockDefinition.Create(k, bb) : bb.Single());
    }
}
