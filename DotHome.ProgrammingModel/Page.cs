using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace DotHome.ProgrammingModel
{
    public class Page : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [JsonIgnore]
        public bool Visible { get; set; } = true;

        [JsonIgnore]
        public double Scale { get; set; } = 1;

        public ObservableCollection<ABlock> Blocks { get; } = new ObservableCollection<ABlock>();

        public ObservableCollection<Wire> Wires { get; } = new ObservableCollection<Wire>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
