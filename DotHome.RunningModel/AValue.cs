using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class AValue
    {
        public abstract Type Type { get; }

        public abstract object ValAsObject { get; }

        public bool Disabled { get; set; }


        public List<AValue> AttachedValues { get; } = new List<AValue>();

        private event Action TransferEvent;

        public void AttachValue(AValue other)
        {
            TransferEvent += RunningModelTools.GetTransferAction(this, other);
            AttachedValues.Add(other);
        }

        public void Transfer()
        {
            TransferEvent?.Invoke();
        }
    }
}
