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
            if(lastS != S.Value)
            {
                lastS = S.Value;
                if(S.Value)
                {
                    Val = true;
                }
            }
            else if (lastR != R.Value)
            {
                lastR = R.Value;
                if (R.Value)
                {
                    Val = false;
                }
            }
            else if (lastT != T.Value)
            {
                lastT = T.Value;
                if (T.Value)
                {
                    Val = !Val;
                }
            }

            if(lastVal != Val)
            {
                lastVal = Val;
                VisualStateHasChanged();
            }

            O.Value = Val;
        }
    }
}
