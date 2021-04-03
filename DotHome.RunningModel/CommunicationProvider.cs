using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Base class for modules communicating with real end device hardware
    /// Even if the channel can be separated for each device (COM port...), The low-level communication code should be executed only in classes derived from <see cref="CommunicationProvider"/> and the methods called from <see cref="GenericDevice.Run"/> should not block.
    /// </summary>
    public abstract class CommunicationProvider
    {
        /// <summary>
        /// Registers device by opening communication channel and creates an entry so that the device values are updated when a message arrives
        /// </summary>
        /// <param name="device"></param>
        public abstract void RegisterDevice(GenericDevice device);

        /// <summary>
        /// Forces writing all (or only changed) values to device
        /// </summary>
        /// <param name="device"></param>
        public abstract void WriteDevice(GenericDevice device);

        /// <summary>
        /// Forces reading all values from device
        /// </summary>
        /// <param name="device"></param>
        public abstract void ReadDevice(GenericDevice device);

        public abstract List<GenericDevice> SearchGenericDevices();
    }

    /// <summary>
    /// Base class for modules communicating with real end device hardware. 
    /// </summary>
    /// <typeparam name="T">The device this class is providing communication for</typeparam>
    public abstract class CommunicationProvider<T> : CommunicationProvider where T : GenericDevice
    {
        protected List<T> devices = new List<T>();

        public override void RegisterDevice(GenericDevice device) => RegisterDevice((T)device);

        public override void WriteDevice(GenericDevice device) => WriteDevice((T)device);

        public override void ReadDevice(GenericDevice device) => ReadDevice((T)device);

        public override List<GenericDevice> SearchGenericDevices() => SearchDevices().Cast<GenericDevice>().ToList();

        public virtual void RegisterDevice(T device)
        {
            devices.Add(device);
        }

        /// <summary>
        /// Forces writing all (or only changed) values to device
        /// </summary>
        /// <param name="device"></param>
        public abstract void WriteDevice(T device);

        /// <summary>
        /// Forces reading all values from device
        /// </summary>
        /// <param name="device"></param>
        public abstract void ReadDevice(T device);

        /// <summary>
        /// Searches for all devices that are connected to this communication channel
        /// </summary>
        /// <returns></returns>
        public abstract List<T> SearchDevices();
    }
}
