using DotHome.RunningModel.Attributes;
using DotHome.RunningModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    public abstract class GenericDevice : Device
    {
        [Parameter]
        public List<GenericDeviceValue> RValues { get; } = new List<GenericDeviceValue>();

        [Parameter]
        public List<GenericDeviceValue> WValues { get; } = new List<GenericDeviceValue>();
    }
}
