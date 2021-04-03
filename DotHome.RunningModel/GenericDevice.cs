using DotHome.RunningModel.Attributes;
using DotHome.RunningModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// A <see cref="Device"/>, composed of generic R- and WValues. According to this values, I/O must be created during activation process.
    /// Init and Run methods are already implemented using generic <see cref="CommunicationProvider"/>. 
    /// Derived class must implement specific parameters (IP Address, COM port, baudrate...) provide corresponding <see cref="CommunicationProvider"/> into constructor
    /// </summary>
    public abstract class GenericDevice : Device
    {
        private CommunicationProvider communicationProvider;

        /// <summary>
        /// Incoming values (sensors)
        /// </summary>
        [Parameter]
        public List<DeviceValue> RValues { get; set; } = new List<DeviceValue>();

        /// <summary>
        /// Outcoming values (actuators)
        /// </summary>
        [Parameter]
        public List<DeviceValue> WValues { get; set; } = new List<DeviceValue>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="communicationProvider">The injected comunication module, providing communication for this particular type of <see cref="GenericDevice"/></param>
        public GenericDevice(CommunicationProvider communicationProvider)
        {
            this.communicationProvider = communicationProvider;
        }

        /// <summary>
        /// Registers device to <see cref="CommunicationProvider"/>
        /// </summary>
        public override void Init()
        {
            communicationProvider.RegisterDevice(this);
        }

        /// <summary>
        /// Transfers <see cref="RValues"/> to <see cref="Block.Outputs"/> and <see cref="WValues"/> to <see cref="Block.Inputs"/>.
        /// If anything changed, forces write to device
        /// </summary>
        public override void Run()
        {
            lock (this)
            {
                for (int i = 0; i < RValues.Count; i++)
                {
                    switch(RValues[i].ValueType)
                    {
                        case DeviceValueType.Pulse:
                            ((Output<bool>)Outputs[i]).Value = RValues[i].Bool;
                            RValues[i].Bool = false;
                            break;
                        case DeviceValueType.Bool:
                            ((Output<bool>)Outputs[i]).Value = RValues[i].Bool;
                            break;
                        case DeviceValueType.Uint8:
                        case DeviceValueType.Uint16:
                        case DeviceValueType.Uint32:
                            ((Output<uint>)Outputs[i]).Value = RValues[i].Uint;
                            break;
                        case DeviceValueType.Int8:
                        case DeviceValueType.Int16:
                        case DeviceValueType.Int32:
                            ((Output<int>)Outputs[i]).Value = RValues[i].Int;
                            break;
                        case DeviceValueType.Float2:
                        case DeviceValueType.Float4:
                        case DeviceValueType.Float:
                            ((Output<double>)Outputs[i]).Value = RValues[i].Float;
                            break;
                        case DeviceValueType.String:
                            ((Output<string>)Outputs[i]).Value = RValues[i].String;
                            break;
                        case DeviceValueType.Binary:
                            ((Output<byte[]>)Outputs[i]).Value = RValues[i].Object;
                            break;
                    }
                }
                bool shouldWrite = false;
                for (int i = 0; i < WValues.Count; i++)
                {
                    switch (WValues[i].ValueType)
                    {
                        case DeviceValueType.Bool:
                            if (WValues[i].Bool != ((Input<bool>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Bool = ((Input<bool>)Inputs[i]).Value;
                            break;
                        case DeviceValueType.Uint8:
                        case DeviceValueType.Uint16:
                        case DeviceValueType.Uint32:
                            if (WValues[i].Uint != ((Input<uint>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Uint = ((Input<uint>)Inputs[i]).Value;
                            break;
                        case DeviceValueType.Int8:
                        case DeviceValueType.Int16:
                        case DeviceValueType.Int32:
                            if (WValues[i].Int != ((Input<int>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Int = ((Input<int>)Inputs[i]).Value;
                            break;
                        case DeviceValueType.Float2:
                        case DeviceValueType.Float4:
                        case DeviceValueType.Float:
                            if (WValues[i].Float != ((Input<double>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Float = (float)((Input<double>)Inputs[i]).Value;
                            break;
                        case DeviceValueType.String:
                            if (WValues[i].String != ((Input<string>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].String = ((Input<string>)Inputs[i]).Value;
                            break;
                        case DeviceValueType.Binary:
                            if (WValues[i].Object != ((Input<byte[]>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Object = ((Input<byte[]>)Inputs[i]).Value;
                            break;
                    }
                }
                if (shouldWrite)
                {
                    Debug.WriteLine("should write");
                    communicationProvider.WriteDevice(this);
                }
            }
        }
    }
}
