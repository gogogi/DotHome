using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.ProgrammingModel.Tools
{
    public static class ProgrammingModelTools
    {
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
