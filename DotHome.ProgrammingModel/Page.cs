using DotHome.Definitions;
using DotHome.ProgrammingModel.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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

        [JsonIgnore]
        public double XOffset { get; set; }

        [JsonIgnore]
        public Point point { get; set; }

        public ObservableCollection<Block> Blocks { get; } = new ObservableCollection<Block>();

        public ObservableCollection<Wire> Wires { get; } = new ObservableCollection<Wire>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Page()
        {
            Blocks.CollectionChanged += Blocks_CollectionChanged;
        }

        public void AddWire(Wire wire)
        {
            var other = Wires.SingleOrDefault(o => wire.Input == o.Input);
            if (other != null) Wires.Remove(other);
            Wires.Add(wire);
        }

        private void Blocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(Block b in e.OldItems)
                {
                    var wiresToRemove = Wires.Where(w => b.Inputs.Contains(w.Input) || b.Outputs.Contains(w.Output)).ToArray();
                    foreach (Wire w in wiresToRemove) Wires.Remove(w);
                }
            }
        }
    }
}
