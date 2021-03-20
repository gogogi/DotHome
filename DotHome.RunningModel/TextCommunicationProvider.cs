using DotHome.RunningModel.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public abstract class TextCommunicationProvider<T> : CommunicationProvider<T> where T : GenericDevice
    {
        protected abstract event Action<string, GenericDevice> TextReceived;
        
        protected abstract void SendText(string text, GenericDevice target);

        public TextCommunicationProvider() 
        {
            TextReceived += TextCommunicationProvider_TextReceived;
        }

        private void TextCommunicationProvider_TextReceived(string text, GenericDevice device)
        {
            if(text.StartsWith("ChangedInfo "))
            {
                try
                {
                    lock(device)
                    {
                        Parse(text.Substring("ChangedInfo ".Length), device);
                    }
                }
                catch
                {

                }
            }
        }

        public override void WriteDevice(GenericDevice device)
        {
            StringBuilder sb = new StringBuilder("Write { ");
            foreach(var v in device.WValues)
            {
                sb.Append("\"");
                sb.Append(v.Name);
                sb.Append("\"");
                sb.Append(" : ");
                switch(v.ValueType)
                {
                    case DeviceValueType.Bool:
                        sb.Append(v.Bool ? "true" : "false");
                        break;
                    case DeviceValueType.Uint8:
                    case DeviceValueType.Uint16:
                    case DeviceValueType.Uint32:
                        sb.Append(v.Uint);
                        break;
                    case DeviceValueType.Int8:
                    case DeviceValueType.Int16:
                    case DeviceValueType.Int32:
                        sb.Append(v.Int);
                        break;
                    case DeviceValueType.Float2:
                    case DeviceValueType.Float4:
                    case DeviceValueType.Float:
                        sb.Append(v.Float);
                        break;
                    case DeviceValueType.String:
                        sb.Append("\"");
                        sb.Append(v.String);
                        sb.Append("\"");
                        break;
                    case DeviceValueType.Object:
                        sb.Append("\"");
                        sb.Append(v.Object == null ? "" : BitConverter.ToString(v.Object).Replace("-", ""));
                        sb.Append("\"");
                        break;
                }
                sb.Append(", ");
            }
            sb.Append("}");
            SendText(sb.ToString(), device);
        }

        public override void ReadDevice(GenericDevice device)
        {
            SendText("Read", null);
        }

        public override List<GenericDevice> SearchDevices()
        {
            var searchDevices = new List<GenericDevice>();

            void handler(string text, GenericDevice device)
            {
                if (text.StartsWith("DetailsResponse"))
                {
                    try
                    {
                        Parse(text.Substring("DetailsResponse ".Length), device);
                        searchDevices.Add(device);
                    }
                    catch
                    {

                    }
                }
            }

            TextReceived += handler;
            SendText("Details", null);
            Thread.Sleep(5000);
            TextReceived -= handler;
            return searchDevices;
        }


        private void Parse(string json, GenericDevice device)
        {
            JObject o = JObject.Parse(json);
            device.Name = o.Value<string>("Name");
            foreach (var rval in o["RValues"].OfType<JProperty>())
            {
                DeviceValue value = device.RValues.SingleOrDefault(v => v.Name == rval.Name);
                bool newVal = (value == null);
                if(newVal)
                {
                    value = new DeviceValue()
                    {
                        Name = rval.Name,
                        ValueType = Enum.Parse<DeviceValueType>(((JArray)rval.Value).Value<string>(0)),
                        State = Enum.Parse<DeviceValueState>(((JArray)rval.Value).Value<string>(1)),
                    };
                }
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
                if(newVal) device.RValues.Add(value);
            }
            foreach (var rval in o["WValues"].OfType<JProperty>())
            {
                DeviceValue value = device.WValues.SingleOrDefault(v => v.Name == rval.Name);
                bool newVal = (value == null);
                if (newVal)
                {
                    value = new DeviceValue()
                    {
                        Name = rval.Name,
                        ValueType = Enum.Parse<DeviceValueType>(((JArray)rval.Value).Value<string>(0)),
                        State = Enum.Parse<DeviceValueState>(((JArray)rval.Value).Value<string>(1)),
                    };
                }
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
                if(newVal) device.WValues.Add(value);
            }
        }
    }
}
