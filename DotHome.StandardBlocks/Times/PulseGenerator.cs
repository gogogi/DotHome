using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        [Parameter(true), Description("Time On [ms]")]
        public int TimeOn { get; set; } = 1000;

        [Parameter(true), Description("Time Off [ms]")]
        public int TimeOff { get; set; } = 1000;
    }
}
