using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Model
{
    public class User : INotifyPropertyChanged
    { 
        private string name, password;
        public string Name { get => name; set { name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); } }

        public string Password { get => password; set { password = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password))); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}
