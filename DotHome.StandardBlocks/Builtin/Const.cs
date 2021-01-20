using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace DotHome.StandardBlocks.Builtin
{
    [Description("Provides constant value"), Category("Builtin"), Color("DarkGray")]
    class Const : ABlock
    {
        [Description("Output")]
        public Output<object> O { get; set; }

        [Parameter(true)]
        public object Value { get; set; } = 0;

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
