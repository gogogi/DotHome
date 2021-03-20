using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Compare
{
    [Category("Compare"), Description("Output is true if I1 > I2")]
    class Greater : Block
    {
        public Input<double> I1 { get; set; }

        public Input<double> I2 { get; set; }

        public Output<bool> O { get; set; }
        public override void Init()
        {
        }

        public override void Run()
        {
            O.Value = (I1.Value > I2.Value);
        }
    }
}
