using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class BlockValue
    {
        public abstract Type Type { get; }
        public abstract object ValAsObject { get; }
        public bool Disabled { get; set; }
        public List<BlockValue> AttachedValues { get; } = new List<BlockValue>();

        private event Action TransferEvent;

        public void AttachValue(BlockValue other)
        {
            TransferEvent += RunningModelTools.GetTransferAction(this, other);
            AttachedValues.Add(other);
        }

        public void Transfer()
        {
            TransferEvent?.Invoke();
        }
    }

    public class BlockValue<T> : BlockValue
    {
        public T Value { get; set; }
        public override Type Type => typeof(T);

        public override object ValAsObject => Value;
    }

    public class Input<T> : BlockValue<T>
    {
    }

    public class Output<T> : BlockValue<T>
    {
    }
}
