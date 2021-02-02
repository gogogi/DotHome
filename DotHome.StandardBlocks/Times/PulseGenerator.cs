using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace DotHome.StandardBlocks.Times
{
    [Description("Generates pulses on output"), Category("Times")]
    public class PulseGenerator : ANamedBlock
    {
        [Description("Disable")]
        public Input<bool> D { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        [Parameter(true), Description("Time On [ms]")]
        public int TimeOn { get; set; } = 1000;

        [Parameter(true), Description("Time Off [ms]")]
        public int TimeOff { get; set; } = 1000;

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
