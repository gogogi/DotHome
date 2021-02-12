using DotHome.Model.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DotHome.Model
{
    public abstract class Value : INotifyPropertyChanged
    {
        private bool disabled;

        public abstract Type Type { get; }

        public abstract object ValAsObject { get; set; }

        public bool Disabled { get => disabled; set => SetAndRaise(ref disabled, value, nameof(Disabled)); }


        public List<Value> AttachedValues { get; } = new List<Value>();

        public event PropertyChangedEventHandler PropertyChanged;
        private event Action TransferEvent;

        public void AttachValue(Value other)
        {
            TransferEvent += ModelTools.GetTransferAction(this, other);
            AttachedValues.Add(other);
        }

        public void Transfer()
        {
            TransferEvent?.Invoke();
        }

        private void SetAndRaise<T>(ref T field, T value, string name)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class Value<T> : Value
    {
        public T Val { get; set; }
        public override Type Type => typeof(T);

        public override object ValAsObject { get => Val; set => Val = (T)value; }
    }
}
