using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Controls
{
    [Category("Controls"), Description("User can switch the value ON/OFF"), Color("#8040fe")]
    public class Switch : VisualBlock
    {
        internal bool Val { get; set; }

        [Description("Set"), Disablable(true)]
        public Input<bool> S { get; set; }

        [Description("Reset"), Disablable(true)]
        public Input<bool> R { get; set; }

        [Description("Toggle"), Disablable(true)]
        public Input<bool> T { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        private bool lastS, lastR, lastT, lastVal;

        public override void Init()
        {

        }

        public override void Run()
        {
            if(lastS != S.Val)
            {
                lastS = S.Val;
                if(S.Val)
                {
                    Val = true;
                }
            }
            else if (lastR != R.Val)
            {
                lastR = R.Val;
                if (R.Val)
                {
                    Val = false;
                }
            }
            else if (lastT != T.Val)
            {
                lastT = T.Val;
                if (T.Val)
                {
                    Val = !Val;
                }
            }

            if(lastVal != Val)
            {
                lastVal = Val;
                VisualStateHasChanged();
            }

            O.Val = Val;
        }
    }
}
