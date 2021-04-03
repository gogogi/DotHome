using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Represents category of automation (Light, Heating, Ventilation...)
    /// </summary>
    public class Category : INotifyPropertyChanged
    {
        private string name;
        public string Name { get => name; set { name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); } }

        public event PropertyChangedEventHandler PropertyChanged;

        //Is overriden for Config gui to work properly
        public override string ToString()
        {
            return Name;
        }
    }
}
