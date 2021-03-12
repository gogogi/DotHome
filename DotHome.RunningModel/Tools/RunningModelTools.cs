using DotHome.RunningModel.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Tools
{
    public static class RunningModelTools
    {
        private static Dictionary<DeviceValueType, Type> deviceValueTypeToTypeDictionary = new Dictionary<DeviceValueType, Type>()
        {
            [DeviceValueType.Bool] = typeof(bool),
            [DeviceValueType.Uint8] = typeof(uint),
            [DeviceValueType.Uint16] = typeof(uint),
            [DeviceValueType.Uint32] = typeof(uint),
            [DeviceValueType.Int8] = typeof(int),
            [DeviceValueType.Int16] = typeof(int),
            [DeviceValueType.Int32] = typeof(int),
            [DeviceValueType.Float2] = typeof(float),
            [DeviceValueType.Float4] = typeof(float),
            [DeviceValueType.Float] = typeof(float),
            [DeviceValueType.String] = typeof(string),
            [DeviceValueType.Object] = typeof(byte[]),
        };

        public static Type[] SupportedTypes { get; } = { typeof(bool), typeof(int), typeof(uint), typeof(double), typeof(string), typeof(byte[]) };

        public static bool IsConversionSupported(Type first, Type second) => GetTransferAction((BlockValue)Activator.CreateInstance(typeof(BlockValue<>).MakeGenericType(first)), (BlockValue)Activator.CreateInstance(typeof(BlockValue<>).MakeGenericType(second))) != null;

        public static Action GetTransferAction(BlockValue first, BlockValue second)
        {
            if (first is BlockValue<bool> vb1)
            {
                if (second is BlockValue<bool> vb2) return () => vb2.Value = vb1.Value;
                else if (second is BlockValue<int> vi2) return () => vi2.Value = vb1.Value ? 1 : 0;
                else if (second is BlockValue<uint> vu2) return () => vu2.Value = (uint)(vb1.Value ? 1 : 0);
                else if (second is BlockValue<double> vd2) return () => vd2.Value = vb1.Value ? 1 : 0;
                else if (second is BlockValue<string> vs2) return () => vs2.Value = vb1.Value.ToString();
            }
            else if (first is BlockValue<int> vi1)
            {
                if (second is BlockValue<bool> vb2) return () => vb2.Value = vi1.Value != 0;
                else if (second is BlockValue<int> vi2) return () => vi2.Value = vi1.Value;
                else if (second is BlockValue<uint> vu2) return () => vu2.Value = (uint)vi1.Value;
                else if (second is BlockValue<double> vd2) return () => vd2.Value = vi1.Value;
                else if (second is BlockValue<string> vs2) return () => vs2.Value = vi1.Value.ToString();
            }
            if (first is BlockValue<uint> vu1)
            {
                if (second is BlockValue<bool> vb2) return () => vb2.Value = vu1.Value != 0;
                else if (second is BlockValue<int> vi2) return () => vi2.Value = (int)vu1.Value;
                else if (second is BlockValue<uint> vu2) return () => vu2.Value = vu1.Value;
                else if (second is BlockValue<double> vd2) return () => vd2.Value = vu1.Value;
                else if (second is BlockValue<string> vs2) return () => vs2.Value = vu1.Value.ToString();
            }
            if (first is BlockValue<double> vd1)
            {
                if (second is BlockValue<bool> vb2) return () => vb2.Value = Math.Abs(vd1.Value) > 1e-6;
                else if (second is BlockValue<int> vi2) return () => vi2.Value = (int)vd1.Value;
                else if (second is BlockValue<uint> vu2) return () => vu2.Value = (uint)vd1.Value;
                else if (second is BlockValue<double> vd2) return () => vd2.Value = vd1.Value;
                else if (second is BlockValue<string> vs2) return () => vs2.Value = vd1.Value.ToString();
            }
            if (first is BlockValue<string> vs1)
            {
                if (second is BlockValue<string> vs2) return () => vs2.Value = vs1.Value;
            }
            if (first is BlockValue<byte[]> vba1)
            {
                if (second is BlockValue<byte[]> vba2) return () => vba2.Value = vba1.Value;
            }
            return null;
        }

        public static Type ToType(this DeviceValueType deviceValueType)
        {
            return deviceValueTypeToTypeDictionary[deviceValueType];
        }
    }
}
