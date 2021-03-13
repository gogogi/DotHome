using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    public abstract class CommunicationProvider<T> where T : GenericDevice
    {
        public abstract List<T> SearchDevices();
    }
}
