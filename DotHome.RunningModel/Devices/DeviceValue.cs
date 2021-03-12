using DotHome.RunningModel.Devices;
using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    public abstract class DeviceValue
    {
        public DeviceValueState State { get; set; }

        public abstract Type Type { get; }
        //public abstract object ValAsObject { get; }
    }

    public class GenericDeviceValue : DeviceValue
    {
        public string Name { get; set; }

        public DeviceValueType GenericType { get; set; }

        public override Type Type => GenericType.ToType();

        public bool Bool { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public float Float { get; set; }
        public string String { get; set; }
        public byte[] Object { get; set; }

        //public override object ValAsObject => throw new NotImplementedException();
    }

    public class DeviceValue<T> : DeviceValue
    {
        public override Type Type => typeof(T);

        //public override object ValAsObject => Value;

        public T Value { get; set; }
    }

    public class RValue<T> : DeviceValue<T>
    {

    }

    public class WValue<T> : DeviceValue<T>
    {

    }
}
