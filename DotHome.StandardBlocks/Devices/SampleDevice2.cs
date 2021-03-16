using DotHome.RunningModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class SampleDevice2 : HardcodedDevice
    {
        public Output<bool> Tlacitko1 { get; set; }

        public Output<bool> Tlacitko2 { get; set; }

        public Input<bool> Kontakt1 { get; set; }

        public Input<bool> Kontakt2 { get; set; }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
