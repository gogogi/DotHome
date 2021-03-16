using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Block = DotHome.ProgrammingModel.Block;

namespace DotHome.Config.Windows
{
    public class GenericDeviceWindow : Window, INotifyPropertyChanged
    {
        private DeviceValue selectedRValue, selectedWValue;
        private Block block;

        private ObservableCollection<DeviceValue> RValues { get; } = new ObservableCollection<DeviceValue>();
        private ObservableCollection<DeviceValue> WValues { get; } = new ObservableCollection<DeviceValue>();

        private DeviceValue SelectedRValue { get => selectedRValue; set { selectedRValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRValue))); } }
        private DeviceValue SelectedWValue { get => selectedWValue; set { selectedWValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedWValue))); } }

        public new event PropertyChangedEventHandler PropertyChanged;

        public GenericDeviceWindow(Block block) : this()
        {
            this.block = block;

            List<DeviceValue> rValues = (List<DeviceValue>)block.Parameters.Single(p => p.Definition.Name == nameof(GenericDevice.RValues)).Value;
            foreach(var v in rValues)
            {
                RValues.Add(v);
            }

            List<DeviceValue> wValues = (List<DeviceValue>)block.Parameters.Single(p => p.Definition.Name == nameof(GenericDevice.WValues)).Value;
            foreach (var v in wValues)
            {
                WValues.Add(v);
            }
        }

        public GenericDeviceWindow()
        {
            DataContext = this;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ButtonAddRValue_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var val = new DeviceValue();
            if (SelectedRValue != null)
            {
                RValues.Insert(RValues.IndexOf(SelectedRValue), val);
            }
            else
            {
                RValues.Add(val);
            }
            SelectedRValue = val;
        }

        private void ButtonRemoveRValue_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int i = RValues.IndexOf(SelectedRValue);
            var list = RValues.ToList();
            list.Remove(SelectedRValue);
            RValues.Clear();
            foreach (var v in list) RValues.Add(v);
            if (RValues.Count > 0) SelectedRValue = RValues[Math.Min(i, RValues.Count - 1)];
        }

        private void ButtonAddWValue_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var val = new DeviceValue();
            if (SelectedWValue != null)
            {
                WValues.Insert(WValues.IndexOf(SelectedWValue), val);
            }
            else
            {
                WValues.Add(val);
            }
            SelectedWValue = val;
        }

        private void ButtonRemoveWValue_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int i = WValues.IndexOf(SelectedWValue);
            var list = WValues.ToList();
            list.Remove(SelectedWValue);
            WValues.Clear();
            foreach (var v in list) WValues.Add(v);
            if (WValues.Count > 0) SelectedWValue = WValues[Math.Min(i, WValues.Count - 1)];
        }

        private void ButtonWValueUp_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int index = WValues.IndexOf(SelectedWValue);
            if (index > 0)
            {
                var tmp = SelectedWValue;
                SelectedWValue = null;
                WValues.Move(index, index - 1);
                var array = WValues.ToArray();
                WValues.Clear();
                foreach (var val in array) WValues.Add(val);
                SelectedWValue = tmp;
            }
        }

        private void ButtonWValueDown_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int index = WValues.IndexOf(SelectedWValue);
            if (index < WValues.Count - 1)
            {
                var tmp = SelectedWValue;
                SelectedWValue = null;
                WValues.Move(index, index + 1);
                var array = WValues.ToArray();
                WValues.Clear();
                foreach (var val in array) WValues.Add(val);
                SelectedWValue = tmp;
            }
        }

        private void ButtonRValueUp_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int index = RValues.IndexOf(SelectedRValue);
            if (index > 0) 
            {
                var tmp = SelectedRValue;
                SelectedRValue = null;
                RValues.Move(index, index - 1);
                var array = RValues.ToArray();
                RValues.Clear();
                foreach (var val in array) RValues.Add(val);
                SelectedRValue = tmp;
            }
        }

        private void ButtonRValueDown_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int index = RValues.IndexOf(SelectedRValue);
            if (index < RValues.Count - 1)
            {
                var tmp = SelectedRValue;
                SelectedRValue = null;
                RValues.Move(index, index + 1);
                var array = RValues.ToArray();
                RValues.Clear();
                foreach (var val in array) RValues.Add(val);
                SelectedRValue = tmp;
            }
        }

        private void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            block.Parameters.Single(p => p.Definition.Name == nameof(GenericDevice.RValues)).Value = RValues.ToList();
            block.Parameters.Single(p => p.Definition.Name == nameof(GenericDevice.WValues)).Value = WValues.ToList();
            if (RValues.GroupBy(v => v.Name).Count() != RValues.Count)
            {
                return;
            }
            if (WValues.GroupBy(v => v.Name).Count() != WValues.Count)
            {
                return;
            }
            BlockDefinition blockDefinition = new BlockDefinition() { Name = block.Definition.Name, Description = block.Definition.Description, Color = block.Definition.Color, Type = block.Definition.Type };
            foreach(var p in block.Definition.Parameters)
            {
                blockDefinition.Parameters.Add(p);
            }
            foreach(var v in RValues)
            {
                blockDefinition.Outputs.Add(new OutputDefinition() { Name = v.Name, Type = v.Type, Disablable = false, DefaultDisabled = false });
            }
            foreach (var v in WValues)
            {
                blockDefinition.Inputs.Add(new InputDefinition() { Name = v.Name, Type = v.Type, Disablable = false, DefaultDisabled = false });
            }
            block.Definition = blockDefinition;

            // Now synchronize I/O with definition
            var inputsNotInDef = block.Inputs.Where(i => !blockDefinition.Inputs.Any(id => id.Name == i.Definition.Name)).ToArray();
            foreach (var i in inputsNotInDef) block.Inputs.Remove(i);
            for(int i = 0; i < blockDefinition.Inputs.Count; i++)
            {
                if (block.Inputs.Count > i && block.Inputs[i].Definition.Name == blockDefinition.Inputs[i].Name) continue; // match
                else
                {
                    var old = block.Inputs.SingleOrDefault(inp => inp.Definition.Name == blockDefinition.Inputs[i].Name);
                    if (old != null)
                    {
                        block.Inputs.Move(block.Inputs.IndexOf(old), i);
                    }
                    else
                    {
                        block.Inputs.Insert(i, new Input(blockDefinition.Inputs[i]));
                    }
                }
            }
            while (block.Inputs.Count > blockDefinition.Inputs.Count) block.Inputs.RemoveAt(block.Inputs.Count - 1); // remove the rest

            var outputsNotInDef = block.Outputs.Where(i => !blockDefinition.Outputs.Any(id => id.Name == i.Definition.Name)).ToArray();
            foreach (var i in outputsNotInDef) block.Outputs.Remove(i);
            for (int i = 0; i < blockDefinition.Outputs.Count; i++)
            {
                if (block.Outputs.Count > i && block.Outputs[i].Definition.Name == blockDefinition.Outputs[i].Name) continue; // match
                else
                {
                    var old = block.Outputs.SingleOrDefault(inp => inp.Definition.Name == blockDefinition.Outputs[i].Name);
                    if (old != null)
                    {
                        block.Outputs.Move(block.Outputs.IndexOf(old), i);
                    }
                    else
                    {
                        block.Outputs.Insert(i, new Output(blockDefinition.Outputs[i]));
                    }
                }
            }
            while (block.Outputs.Count > blockDefinition.Outputs.Count) block.Outputs.RemoveAt(block.Outputs.Count - 1); // remove the rest
            Close(true);
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
