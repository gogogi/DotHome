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
        protected abstract event Action<string> TextReceived;
        
        protected abstract void SendText(string text, GenericDevice target);

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

    }
}
