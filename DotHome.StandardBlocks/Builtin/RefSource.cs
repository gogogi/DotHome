using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Builtin
{
    [Description("Provides value from corresponding RefSinks"), Category("Builtin"), Color("Green")]
    class RefSource : ABlock
    {
        [Description("Output")]
        public Output<object> O { get; set; }

        [Parameter(true)]
        public string Reference { get; set; } = "Ref";

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
