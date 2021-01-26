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
        public ObservableCollection<Block> SelectedBlocks { get; } = new ObservableCollection<Block>();

        [JsonIgnore]
        public Block SelectedBlock { get; private set; }

        public ObservableCollection<Block> Blocks { get; } = new ObservableCollection<Block>();

        public ObservableCollection<Wire> Wires { get; } = new ObservableCollection<Wire>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Page()
        {
            Blocks.CollectionChanged += Blocks_CollectionChanged;
            SelectedBlocks.CollectionChanged += SelectedBlocks_CollectionChanged;
        }

        private void SelectedBlocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedBlocks.Count == 1) SelectedBlock = SelectedBlocks[0];
            else SelectedBlock = null;
        }

        public void AddWire(Wire wire)
        {
            if (!Blocks.Any(b => b.Inputs.Contains(wire.Input) && b.Outputs.Contains(wire.Output)))
            {
                var other = Wires.SingleOrDefault(o => wire.Input == o.Input);
                if (other != null) Wires.Remove(other);
                Wires.Add(wire);
            }
        }

        private void Blocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(Block b in e.OldItems)
                {
                    var wiresToRemove = Wires.Where(w => b.Inputs.Contains(w.Input) || b.Outputs.Contains(w.Output)).ToArray();
                    foreach (Wire w in wiresToRemove) Wires.Remove(w);
                    if (b.Selected) SelectedBlocks.Remove(b);
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Block b in e.NewItems)
                {
                    if (b.Selected) SelectedBlocks.Add(b);
                    b.PropertyChanged += Block_PropertyChanged;
                    foreach(Input i in b.Inputs)
                    {
                        i.PropertyChanged += Input_PropertyChanged;
                    }
                    foreach (Output o in b.Outputs)
                    {
                        o.PropertyChanged += Output_PropertyChanged;
                    }
                }
            }

        }

        private void Output_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Input.Disabled))
            {
                Output output = (Output)sender;
                var wiresToRemove = Wires.Where(w => w.Output == output).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        private void Input_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Output.Disabled))
            {
                Input input = (Input)sender;
                var wiresToRemove = Wires.Where(w => w.Input == input).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        private void Block_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Block block = (Block)sender;
            if(e.PropertyName == nameof(Block.Selected))
            {
                if (block.Selected) SelectedBlocks.Add(block);
                else SelectedBlocks.Remove(block);
            }
        }
    }
}
