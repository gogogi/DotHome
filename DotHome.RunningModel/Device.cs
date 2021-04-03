using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// A <see cref="NamedBlock"/> which represents a physical end device. Only this blocks should interact with physical word.
    /// </summary>
    [Color("Red"), Category("Devices")]
    public abstract class Device : NamedBlock
    {
        [Parameter(true)]
        public new string Name { get; set; } = "Device";
    }
}
