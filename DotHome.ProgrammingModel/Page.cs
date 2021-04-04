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
    /// <summary>
    /// Organization unit for Config GUI. <see cref="Project"/> consists of <see cref="Page"/>s, <see cref="Page"/>s consist of <see cref="Block"/>s and <see cref="Wire"/>s
    /// </summary>
    public class Page : INotifyPropertyChanged
    {
        /// <summary>
        /// Page name. Must be unique, is showed in Config GUI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Page width in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Page height in pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Determines if the page is 'open' - it means if it's tab is visible in main window. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Page scale in Config GUI - 1 = 100%. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public double Scale { get; set; } = 1;

        /// <summary>
        /// Selected blocks in a Page. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Block> SelectedBlocks { get; } = new ObservableCollection<Block>();

        /// <summary>
        /// Single selected block in a Page. Is not null only when count of <see cref="SelectedBlocks"/> is exactly 1. Only for visualization bindings.
        /// </summary>
        [JsonIgnore]
        public Block SelectedBlock { get; private set; }

        /// <summary>
        /// <see cref="Block"/>s on this page.
        /// </summary>
        public ObservableCollection<Block> Blocks { get; } = new ObservableCollection<Block>();

        /// <summary>
        /// <see cref="Wire"/>s connecting <see cref="Block.Inputs"/> and <see cref="Block.Outputs"/>
        /// </summary>
        public ObservableCollection<Wire> Wires { get; } = new ObservableCollection<Wire>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Page()
        {
            Blocks.CollectionChanged += Blocks_CollectionChanged;
            SelectedBlocks.CollectionChanged += SelectedBlocks_CollectionChanged;
        }

        /// <summary>
        /// Update <see cref="SelectedBlock"/> according to <see cref="SelectedBlocks"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedBlocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedBlocks.Count == 1) SelectedBlock = SelectedBlocks[0];
            else SelectedBlock = null;
        }

        /// <summary>
        /// Safely adds <see cref="Wire"/>. When <see cref="Wire.Input"/> already has a wire attached, it is removed.
        /// </summary>
        /// <param name="wire"></param>
        public void AddWire(Wire wire)
        {
            if (!Blocks.Any(b => b.Inputs.Contains(wire.Input) && b.Outputs.Contains(wire.Output)))
            {
                var other = Wires.SingleOrDefault(o => wire.Input == o.Input);
                if (other != null) Wires.Remove(other);
                Wires.Add(wire);
            }
        }

        /// <summary>
        /// Keeps consistence between <see cref="Blocks"/> and <see cref="Wires"/>. If a block is removed, all attached wires are also removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            else if(e.Action == NotifyCollectionChangedAction.Add) // Add event handlers for new blocks
            {
                foreach (Block b in e.NewItems)
                {
                    if (b.Selected) SelectedBlocks.Add(b);
                    b.PropertyChanged += Block_PropertyChanged;
                    b.Inputs.CollectionChanged += Inputs_CollectionChanged;
                    b.Outputs.CollectionChanged += Outputs_CollectionChanged;
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

        /// <summary>
        /// If an <see cref="Output"/> was removed, remove all attached <see cref="Wire"/>s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Outputs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                var wiresToRemove = Wires.Where(w => e.OldItems.Contains(w.Output)).ToArray();
                foreach (Wire w in wiresToRemove) Wires.Remove(w);
            }
        }

        /// <summary>
        /// If an <see cref="Input"/> was removed, remove all attached <see cref="Wire"/>s (there should only be one)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inputs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var wiresToRemove = Wires.Where(w => e.OldItems.Contains(w.Input)).ToArray();
                foreach (Wire w in wiresToRemove) Wires.Remove(w);
            }
        }

        /// <summary>
        /// If an <see cref="Output"/> was disabled, remove all attached <see cref="Wire"/>s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Output_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Input.Disabled))
            {
                Output output = (Output)sender;
                var wiresToRemove = Wires.Where(w => w.Output == output).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        /// <summary>
        /// If an <see cref="Input"/> was disabled, remove all attached <see cref="Wire"/>s (there should only be one)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Output.Disabled))
            {
                Input input = (Input)sender;
                var wiresToRemove = Wires.Where(w => w.Input == input).ToArray();
                foreach (Wire wire in wiresToRemove) Wires.Remove(wire);
            }
        }

        /// <summary>
        /// Update <see cref="SelectedBlocks"/> if a <see cref="Block"/> was selected or unselected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
