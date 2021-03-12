using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace DotHome.StandardBlocks.Logics
{
    [Description("Output is negated input"), Category("Logics")]
    class Not : Block
    {
        [Description("Input")]
        public Input<bool> I { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        public override void Init()
        {

        }

        public override void Run()
        {
            O.Value = !I.Value;
        }
    }
}
