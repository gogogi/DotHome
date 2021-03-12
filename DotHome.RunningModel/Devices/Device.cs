using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    [Color("Red"), Category("Devices")]
    public abstract class Device : Block
    {
        [Parameter(true)]
        public string Name { get; set; } = "Device";

        public abstract void ReadValues();

        public abstract void WriteValues();
    }
}
