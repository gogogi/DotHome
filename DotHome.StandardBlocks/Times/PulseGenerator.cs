using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace DotHome.StandardBlocks.Times
{
    [Description("Generates pulses on output"), Category("Times"), Color("Orange")]
    public class PulseGenerator : ANamedBlock
    {
        [Description("Disable")]
        public Input<bool> D { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        [Parameter(true), Description("Time On [ms]"), Range(500, 60 * 60_000)]
        public uint TimeOn { get; set; } = 1000;

        [Parameter(true), Description("Time Off [ms]"), Range(500, 60 * 60_000)]
        public uint TimeOff { get; set; } = 1000;

        private TimeProvider timeProvider;

        public PulseGenerator(TimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public override void Init()
        {
            O.Val = false;
        }

        public override void Run()
        {
            O.Val = !D.Val && (timeProvider.Millis % (TimeOn + TimeOff)) > TimeOff;
        }

        
    }
}
