using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Builtin
{
    [Description("Transfers value to corresponding RefSources"), Category("Builtin"), Color("Green")]
    class RefSink : ABlock
    {
        [Description("Input")]
        public Input<object> I { get; set; }

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
