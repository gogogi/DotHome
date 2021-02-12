using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Model.Tools
{
    public static class ModelTools
    {
        public static Type[] SupportedTypes { get; } = { typeof(bool), typeof(int), typeof(uint), typeof(double), typeof(string), typeof(byte[]) };

        public static bool IsConversionSupported(Type first, Type second) => GetTransferAction((Value)Activator.CreateInstance(typeof(Value<>).MakeGenericType(first)), (Value)Activator.CreateInstance(typeof(Value<>).MakeGenericType(second))) != null;

        public static Action GetTransferAction(Value first, Value second)
        {
            if (first is Value<bool> vb1)
            {
                if (second is Value<bool> vb2) return () => vb2.Val = vb1.Val;
                else if (second is Value<int> vi2) return () => vi2.Val = vb1.Val ? 1 : 0;
                else if (second is Value<uint> vu2) return () => vu2.Val = (uint)(vb1.Val ? 1 : 0);
                else if (second is Value<double> vd2) return () => vd2.Val = vb1.Val ? 1 : 0;
                else if (second is Value<string> vs2) return () => vs2.Val = vb1.Val.ToString();
            }
            else if (first is Value<int> vi1)
            {
                if (second is Value<bool> vb2) return () => vb2.Val = vi1.Val != 0;
                else if (second is Value<int> vi2) return () => vi2.Val = vi1.Val;
                else if (second is Value<uint> vu2) return () => vu2.Val = (uint)vi1.Val;
                else if (second is Value<double> vd2) return () => vd2.Val = vi1.Val;
                else if (second is Value<string> vs2) return () => vs2.Val = vi1.Val.ToString();
            }
            if (first is Value<uint> vu1)
            {
                if (second is Value<bool> vb2) return () => vb2.Val = vu1.Val != 0;
                else if (second is Value<int> vi2) return () => vi2.Val = (int)vu1.Val;
                else if (second is Value<uint> vu2) return () => vu2.Val = vu1.Val;
                else if (second is Value<double> vd2) return () => vd2.Val = vu1.Val;
                else if (second is Value<string> vs2) return () => vs2.Val = vu1.Val.ToString();
            }
            if (first is Value<double> vd1)
            {
                if (second is Value<bool> vb2) return () => vb2.Val = Math.Abs(vd1.Val) > 1e-6;
                else if (second is Value<int> vi2) return () => vi2.Val = (int)vd1.Val;
                else if (second is Value<uint> vu2) return () => vu2.Val = (uint)vd1.Val;
                else if (second is Value<double> vd2) return () => vd2.Val = vd1.Val;
                else if (second is Value<string> vs2) return () => vs2.Val = vd1.Val.ToString();
            }
            if (first is Value<string> vs1)
            {
                if (second is Value<string> vs2) return () => vs2.Val = vs1.Val;
            }
            if (first is Value<byte[]> vba1)
            {
                if (second is Value<byte[]> vba2) return () => vba2.Val = vba1.Val;
            }
            return null;
        }

        public static void SetBlockIds(Project project)
        {
            int id = 1;
            foreach(Page page in project.Pages)
            {
                foreach(Block block in page.Blocks)
                {
                    block.Id = id++;
                }
            }
        }
    }
}
