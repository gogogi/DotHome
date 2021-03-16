using DotHome.RunningModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Tools
{
    public static class GenericDeviceParser
    {
        public static void Parse(string json, GenericDevice device)
        {
            JObject o = JObject.Parse(json);
            device.Name = o.Value<string>("Name");
            foreach(var rval in o["RValues"].OfType<JProperty>())
            {
                DeviceValue value = new DeviceValue()
                {
                    Name = rval.Name, 
                    ValueType = Enum.Parse<DeviceValueType>(((JArray)rval.Value).Value<string>(0)),
                    State = Enum.Parse<DeviceValueState>(((JArray)rval.Value).Value<string>(1)),
                };
                switch(value.ValueType)
                {
                    case DeviceValueType.Bool:
                        value.Bool = ((JArray)rval.Value).Value<bool>(2);
                        break;
                    case DeviceValueType.Uint8:
                    case DeviceValueType.Uint16:
                    case DeviceValueType.Uint32:
                        value.Uint = ((JArray)rval.Value).Value<uint>(2);
                        break;
                    case DeviceValueType.Int8:
                    case DeviceValueType.Int16:
                    case DeviceValueType.Int32:
                        value.Int = ((JArray)rval.Value).Value<int>(2);
                        break;
                    case DeviceValueType.Float2:
                    case DeviceValueType.Float4:
                    case DeviceValueType.Float:
                        value.Float = ((JArray)rval.Value).Value<float>(2);
                        break;
                    case DeviceValueType.String:
                        value.String = ((JArray)rval.Value).Value<string>(2);
                        break;
                    case DeviceValueType.Object:
                        string s = ((JArray)rval.Value).Value<string>(2);
                        byte[] b = new byte[s.Length / 2];
                        for(int i = 0; i < b.Length; i++)
                        {
                            b[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                        }
                        break;
                }
                device.RValues.Add(value);
            }
            foreach (var rval in o["WValues"].OfType<JProperty>())
            {
                DeviceValue value = new DeviceValue()
                {
                    Name = rval.Name,
                    ValueType = Enum.Parse<DeviceValueType>(((JArray)rval.Value).Value<string>(0)),
                    State = Enum.Parse<DeviceValueState>(((JArray)rval.Value).Value<string>(1)),
                };
                switch (value.ValueType)
                {
                    case DeviceValueType.Bool:
                        value.Bool = ((JArray)rval.Value).Value<bool>(2);
                        break;
                    case DeviceValueType.Uint8:
                    case DeviceValueType.Uint16:
                    case DeviceValueType.Uint32:
                        value.Uint = ((JArray)rval.Value).Value<uint>(2);
                        break;
                    case DeviceValueType.Int8:
                    case DeviceValueType.Int16:
                    case DeviceValueType.Int32:
                        value.Int = ((JArray)rval.Value).Value<int>(2);
                        break;
                    case DeviceValueType.Float2:
                    case DeviceValueType.Float4:
                    case DeviceValueType.Float:
                        value.Float = ((JArray)rval.Value).Value<float>(2);
                        break;
                    case DeviceValueType.String:
                        value.String = ((JArray)rval.Value).Value<string>(2);
                        break;
                    case DeviceValueType.Object:
                        string s = ((JArray)rval.Value).Value<string>(2);
                        byte[] b = new byte[s.Length / 2];
                        for (int i = 0; i < b.Length; i++)
                        {
                            b[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                        }
                        break;
                }
                device.WValues.Add(value);
            }
        }
    }
}
