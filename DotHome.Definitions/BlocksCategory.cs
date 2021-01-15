using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Definitions
{
    public class BlocksCategory
    {
        public string Name { get; set; }
        public List<BlockDefinition> BlockDefinitions { get; } = new List<BlockDefinition>();
    }
}
