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
    [Category("Math"), Description("Output is Input rounded to specified number of decimal places"), Color("#64b464")]
    public class Round : ABlock
    {
        [Description("Input")]
        public Input<double> I { get; set; }

        [Description("Outut")]
        public Output<double> O { get; set; }

        [Parameter, Description("Number of decimal places")]
        public uint DecimalPlaces { get; set; } = 0;

        public override void Init()
        {

        }

        public override void Run()
        {
            O.Val = System.Math.Round(I.Val, (int)DecimalPlaces);
        }
    }
}
