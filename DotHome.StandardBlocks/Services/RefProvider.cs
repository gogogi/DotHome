using DotHome.RunningModel;
using DotHome.StandardBlocks.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Services
{
    public class RefProvider : IBlockService
    {
        public Dictionary<string, RefSink> RefSinks { get; } = new Dictionary<string, RefSink>();

        public Dictionary<string, List<RefSource>> RefSources { get; } = new Dictionary<string, List<RefSource>>();

        public void Init()
        {
            
        }

        public void Run()
        {
            
        }
    }
}
