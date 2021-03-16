using DotHome.RunningModel.Attributes;
using DotHome.RunningModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public abstract class GenericDevice : Device
    {
        private CommunicationProvider communicationProvider;

        [Parameter]
        public List<DeviceValue> RValues { get; set; } = new List<DeviceValue>();

        [Parameter]
        public List<DeviceValue> WValues { get; set; } = new List<DeviceValue>();

        public GenericDevice(CommunicationProvider communicationProvider)
        {
            this.communicationProvider = communicationProvider;
        }

        public override void Init()
        {
            
        }

        public override void Run()
        {
            lock(this)
            {
                for (int i = 0; i < RValues.Count; i++)
                {
                    switch(RValues[i].ValueType)
                    {
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
                        case DeviceValueType.Object:
                            ((Output<byte[]>)Outputs[i]).Value = RValues[i].Object;
                            break;
                    }
                }
                for (int i = 0; i < WValues.Count; i++)
                {
                    bool shouldWrite = false;
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
                        case DeviceValueType.Object:
                            if (WValues[i].Object != ((Input<byte[]>)Inputs[i]).Value) shouldWrite = true;
                            WValues[i].Object = ((Input<byte[]>)Inputs[i]).Value;
                            break;
                    }
                    if(shouldWrite)
                    {
                        communicationProvider.WriteDevice(this);
                    }
                }
            }
        }
    }
}
