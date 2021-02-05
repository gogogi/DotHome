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
    public class FlipFlop : ABlock
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
            if (S.Val && R.Val) O.Val = DominantSet;
            else if (S.Val) O.Val = true;
            else if (R.Val) O.Val = false;
            else if(lastT != T.Val)
            {
                lastT = T.Val;
                if (T.Val) O.Val = !O.Val;
            }
        }
    }
}
