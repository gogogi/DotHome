using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.RunningModel
{
    public class Value<T> : AValue
    {
        public T Val { get; set; }
        public override Type Type => typeof(T);
    }
}
