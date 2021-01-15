using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Times
{
    [Description("Generates pulses on output"), Category("Times")]
    public class PulseGenerator : ABlock
    {
        [Description("Disable")]
        public Input<bool> D { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        [Parameter, Description("Time On [ms]")]
        public int TimeOn { get; set; } = 1000;

        [Parameter, Description("Time Off [ms]")]
        public int TimeOff { get; set; } = 1000;
    }
}
