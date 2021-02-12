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
    class Const<T> : Block
    {
        [Description("Output")]
        public Output<T> O { get; set; }

        [BlockParameter(true)]
        public T Value { get; set; } = default;

        public override void Init()
        {
            O.Val = Value;
        }

        public override void Run()
        {
            
        }
    }
}
