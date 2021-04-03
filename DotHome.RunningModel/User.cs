using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// Represents user of the visualisation GUI
    /// </summary>
    public class User : INotifyPropertyChanged
    { 
        private string name, password;
        public string Name { get => name; set { name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); } }

        //TODO hash?
        public string Password { get => password; set { password = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password))); } }

        public event PropertyChangedEventHandler PropertyChanged;

        //Is overriden for Config gui to work properly
        public override string ToString()
        {
            return Name;
        }
    }
}
