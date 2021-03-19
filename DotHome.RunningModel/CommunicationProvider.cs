using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public abstract class CommunicationProvider
    {
        protected List<GenericDevice> devices = new List<GenericDevice>();

        public abstract List<GenericDevice> SearchDevices();

        public abstract void WriteDevice(GenericDevice device);

        public abstract void ReadDevice(GenericDevice device);

        public void RegisterDevice(GenericDevice device)
        {
            devices.Add(device);
        }
    }

    public abstract class CommunicationProvider<T> : CommunicationProvider where T : GenericDevice
    {

    }
}
