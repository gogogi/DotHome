﻿using DotHome.RunningModel;
using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public class DeviceValue
    {
        public string Name { get; set; }

        public DeviceValueState State { get; set; }

        public DeviceValueType ValueType { get; set; }

        public Type Type => ValueType.ToType();

        public bool Bool { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public float Float { get; set; }
        public string String { get; set; }
        public byte[] Object { get; set; }
    }
}
