using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.StandardBlocks.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Times
{
    [Description("This block delays rising edge. Falling edge goes immediately"), Category("Times"), Color("Orange")]
    public class DelayRising : Block
    {
        private TimeProvider timeProvider;

        private bool last, waiting;
        private long timeOn;

        [Description("Input")]
        public Input<bool> I { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        [RunningModel.Attributes.Parameter(true), Description("Time to delay rising edge [ms]"), Range(500, 60 * 60 * 1000)]
        public int Delay { get; set; } = 1000;

        public DelayRising(TimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public override void Init()
        {

        }

        public override void Run()
        {
            if(last != I.Value)
            {
                last = I.Value;
                if(I.Value)
                {
                    timeOn = timeProvider.Millis;
                    waiting = true;
                }
                else
                {
                    O.Value = false;
                    waiting = false;
                }
            }
            if(waiting && timeProvider.Millis > timeOn + Delay)
            {
                O.Value = true;
                waiting = false;
            }
        }
    }
}
