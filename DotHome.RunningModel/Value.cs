using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.RunningModel
{
    public class Value<T> : AValue
    {
        public T Val { get; set; }
        public override Type Type => typeof(T);

        public override object ValAsObject => Val;

        //public override bool TryAttachValue(AValue other)
        //{
        //    if(RunningModelTools.SupportedTypes.Contains(Type) && RunningModelTools.SupportedTypes.Contains(other.Type) && RunningModelTools.SupportedConversions[Type].Contains(other.Type))
        //    {
        //        if(this is Value<bool> vb1)
        //        {
        //            if(other is Value<bool> vb2) 
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
