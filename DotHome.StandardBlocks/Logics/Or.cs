﻿using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Logics
{
    [Description("Output is HIGH if at least one input is HIGH"), Category("Logics")]
    public class Or : Block
    {
        [Description("Input 1")]
        public Input<bool> I1 { get; set; }

        [Description("Input 2")]
        public Input<bool> I2 { get; set; }

        [Disablable(true), Description("Input 3")]
        public Input<bool> I3 { get; set; }

        [Disablable(true), Description("Input 4")]
        public Input<bool> I4 { get; set; }

        [Description("Output")]
        public Output<bool> O { get; set; }

        public override void Init()
        {

        }

        public override void Run()
        {
            O.Value = I1.Value || I2.Value || I3.Value || I4.Value;
        }
    }
}
