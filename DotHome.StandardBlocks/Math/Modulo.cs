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
    [Category("Math"), Description("O = I1 mod I2"), Color("#64b464")]
    public class Modulo : Block
    {
        [Description("Input 1")]
        public Input<double> I1 { get; set; }

        [Description("Input 2")]
        public Input<double> I2 { get; set; }

        [Description("Output")]
        public Output<double> O { get; set; }

        public override void Init()
        {

        }

        public override void Run()
        {
            O.Value = I1.Value % I2.Value;
        }
    }
}
