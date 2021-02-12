using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Model
{
    public class Page : INotifyPropertyChanged
    {
        private string name;
        private int width, height;
        private bool visible;
        private double scale;
        private Block selectedBlock;

        public string Name { get => name; set => SetAndRaise(ref name, value, nameof(Name)); }

        public int Width { get => width; set => SetAndRaise(ref width, value, nameof(Width)); }

        public int Height { get => height; set => SetAndRaise(ref height, value, nameof(Height)); }

        public bool Visible { get => visible; set => SetAndRaise(ref visible, value, nameof(Visible)); }

        public double Scale { get => scale; set => SetAndRaise(ref scale, value, nameof(Scale)); }

        public Block SelectedBlock { get => selectedBlock; set => SetAndRaise(ref selectedBlock, value, nameof(SelectedBlock)); }

        public ObservableCollection<Block> Blocks { get; } = new ObservableCollection<Block>();

        public ObservableCollection<Wire> Wires { get; } = new ObservableCollection<Wire>();

        public ObservableCollection<Block> SelectedBlocks { get; } = new ObservableCollection<Block>();


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
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Block b in e.OldItems)
                {
                    var wiresToRemove = Wires.Where(w => b.Inputs.Contains(w.Input) || b.Outputs.Contains(w.Output)).ToArray();
                    foreach (Wire w in wiresToRemove) Wires.Remove(w);
                    if (b.Selected) SelectedBlocks.Remove(b);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Block b in e.NewItems)
                {
                    if (b.Selected) SelectedBlocks.Add(b);
                    b.PropertyChanged += Block_PropertyChanged;
                    foreach (Value i in b.Inputs)
                    {
                        i.PropertyChanged += Input_PropertyChanged;
                    }
                    foreach (Value o in b.Outputs)
                    {
                        o.PropertyChanged += Output_PropertyChanged;
                    }
                }
            }

        }

        private void Output_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Value.Disabled))
            {
                Value output = (Value)sender;
                var wiresToRemove = Wires.Where(w => w.Output == output).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        private void Input_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Value.Disabled))
            {
                Value input = (Value)sender;
                var wiresToRemove = Wires.Where(w => w.Input == input).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        private void Block_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Block block = (Block)sender;
            if (e.PropertyName == nameof(Block.Selected))
            {
                if (block.Selected) SelectedBlocks.Add(block);
                else SelectedBlocks.Remove(block);
            }
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
}
