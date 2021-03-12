using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Logics
{
    [Category("Logics"), Description("Output is switched HIGH/LOW according to inputs")]
    public class FlipFlop : Block
    {
        [Description("Set")]
        public Input<bool> S { get; set; }

        [Description("Reset")]
        public Input<bool> R { get; set; }

        [Description("Toggle (rising edge)")]
        public Input<bool> T { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        [Parameter, Description("Value on Output if both Set and Reset are HIGH")]
        public bool DominantSet { get; set; } = false;

        private bool lastT;

        public override void Init()
        {
            
        }

        public override void Run()
        {
            if (S.Value && R.Value) O.Value = DominantSet;
            else if (S.Value) O.Value = true;
            else if (R.Value) O.Value = false;
            else if(lastT != T.Value)
            {
                lastT = T.Value;
                if (T.Value) O.Value = !O.Value;
            }
        }
    }
}
