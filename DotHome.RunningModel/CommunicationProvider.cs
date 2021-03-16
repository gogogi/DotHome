using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public abstract class CommunicationProvider
    {
        public abstract List<GenericDevice> SearchDevices();

        public abstract void WriteDevice(GenericDevice device);
    }

    public abstract class CommunicationProvider<T> : CommunicationProvider where T : GenericDevice
    {
        private List<T> devices = new List<T>();
        public abstract override List<GenericDevice> SearchDevices();

        public void RegisterDevice(T device)
        {
            devices.Add(device);
        }
    }
}
