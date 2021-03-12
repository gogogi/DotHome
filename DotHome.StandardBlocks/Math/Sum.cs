using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Math
{
    [Category("Math"), Description("Output is sum of all inputs"), Color("#64b464")]
    public class Sum : Block
    {
        [Description("Input 1")]
        public Input<double> I1 { get; set; }

        [Description("Input 2")]
        public Input<double> I2 { get; set; }

        [Description("Input 3"), Disablable(true)]
        public Input<double> I3 { get; set; }

        [Description("Input 4"), Disablable(true)]
        public Input<double> I4 { get; set; }

        [Description("Output")]
        public Output<double> O { get; set; }

        public override void Init()
        {

        }

        public override void Run()
        {
            O.Value = I1.Value + I2.Value + I3.Value + I4.Value;
        }
    }
}
