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
    [Category("Math"), Description("Output is minimum of all enabled inputs"), Color("#64b464")]
    public class Min : ABlock
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
            double val = System.Math.Min(I1.Val, I2.Val);
            if (!I3.Disabled) val = System.Math.Min(val, I3.Val);
            if (!I4.Disabled) val = System.Math.Min(val, I4.Val);
            O.Val = val;
        }
    }
}
