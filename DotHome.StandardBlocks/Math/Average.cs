using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Math
{
    [Category("Math"), Description("Output is arithmetical average of all enabled inputs"), Color("#64b464")]
    public class Average : Block
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
            O.Val = (I1.Val + I2.Val + I3.Val + I4.Val) / (2 + (I3.Disabled ? 0 : 1) + (I4.Disabled ? 0 : 1));
        }
    }
}
