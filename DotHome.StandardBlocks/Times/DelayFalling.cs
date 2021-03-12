using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Times
{
    [Description("This block delays falling edge. Rising edge goes immediately"), Category("Times"), Color("Orange")]
    public class DelayFalling : Block
    {
        private TimeProvider timeProvider;

        private bool last, waiting;
        private long timeOff;

        [Description("Input")]
        public Input<bool> I { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        [Parameter(true), Description("Time to delay falling edge [ms]"), Range(500, 60 * 60 * 1000)]
        public int Delay { get; set; } = 1000;

        public DelayFalling(TimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public override void Init()
        {

        }

        public override void Run()
        {
            if (last != I.Value)
            {
                last = I.Value;
                if (I.Value)
                {
                    O.Value = true;
                    waiting = false;
                }
                else
                {
                    timeOff = timeProvider.Millis;
                    waiting = true;
                }
            }
            if (waiting && timeProvider.Millis > timeOff + Delay)
            {
                O.Value = false;
                waiting = false;
            }
        }
    }
}
