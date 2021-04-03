using DotHome.RunningModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Base class for containers representing I/O of <see cref="Block"/>s
    /// </summary>
    public abstract class BlockValue
    {
        public abstract Type Type { get; }

        /// <summary>
        /// Provides generic access to value of derived classes for debugging purpose
        /// </summary>
        public abstract object ValueAsObject { get; }
        public bool Disabled { get; set; }
        private List<BlockValue> AttachedValues { get; } = new List<BlockValue>();

        private event Action TransferEvent;

        /// <summary>
        /// Attaches the <paramref name="other"/> so that when <see cref="Transfer"/> is called, its value is updated with current value with appropriate conversion
        /// </summary>
        /// <param name="other"></param>
        public void AttachValue(BlockValue other)
        {
            TransferEvent += RunningModelTools.GetTransferAction(this, other);
            AttachedValues.Add(other);
        }

        /// <summary>
        /// Updates values registered with <see cref="AttachValue"/>
        /// </summary>
        public void Transfer()
        {
            TransferEvent?.Invoke();
        }
    }

    /// <summary>
    /// Generic base class for containers representing I/O of <see cref="Block"/>s
    /// Note that supported types for <typeparamref name="T"/> are <see cref="bool"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="double"/>, <see cref="string"/> and <see cref="byte[]"/>
    /// </summary>
    public class BlockValue<T> : BlockValue
    {
        public T Value { get; set; }
        public override Type Type => typeof(T);

        public override object ValueAsObject => Value;
    }

    /// <summary>
    /// Input (Incoming value) of a <see cref="Block"/>
    /// It is the same as <see cref="BlockValue{T}"/> and <see cref="Output{T}"/> but we need to distinguish I/O somehow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Input<T> : BlockValue<T>
    {
    }

    /// <summary>
    /// Output (Outcoming value) of a <see cref="Block"/>
    /// It is the same as <see cref="BlockValue{T}"/> and <see cref="Input{T}"/> but we need to distinguish I/O somehow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Output<T> : BlockValue<T>
    {
    }
}
