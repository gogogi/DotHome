using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.ProgrammingModel.Tools
{
    /// <summary>
    /// Helper class for namespace <see cref="ProgrammingModel"/>
    /// </summary>
    public static class ProgrammingModelTools
    {
        /// <summary>
        /// Sets unique (1 - based) <see cref="Block.Id"/> for each <see cref="Block"/> in <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        public static void SetBlockIDs(Project project)
        {
            int id = 1;
            foreach(Page page in project.Pages)
            {
                foreach(Block block in page.Blocks)
                {
                    block.Id = id++;
                }
            }
        }
    }
}
