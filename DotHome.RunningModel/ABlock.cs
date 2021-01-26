using System;
using System.Collections.Generic;

namespace DotHome.RunningModel
{
    public abstract class ABlock
    {
        public abstract void Init();
        public abstract void Run();

        public List<AValue> Inputs { get; } = new List<AValue>();
        public List<AValue> Outputs { get; } = new List<AValue>();

        //public ABlock()
        //{

        //}
    }
}
