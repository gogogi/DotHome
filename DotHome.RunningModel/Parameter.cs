using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Model
{
    public abstract class Parameter : INotifyPropertyChanged
    {
        public abstract event PropertyChangedEventHandler PropertyChanged;
    }

    public class Parameter<T> : Parameter
    {
        private T value;

        public T Value { get => value; set => SetAndRaise(ref this.value, value, nameof(Value)); }

        public Parameter(T value)
        {
            this.value = value;
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        private void SetAndRaise<T>(ref T field, T value, string name)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
