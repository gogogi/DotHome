using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.ProgrammingModel;
using DotHome.RunningModel.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class GenericDeviceWindow : Window, INotifyPropertyChanged
    {
        private GenericDeviceValue selectedRValue, selectedWValue;
        private Block block;

        private ObservableCollection<GenericDeviceValue> RValues { get; } = new ObservableCollection<GenericDeviceValue>();
        private ObservableCollection<GenericDeviceValue> WValues { get; } = new ObservableCollection<GenericDeviceValue>();

        private GenericDeviceValue SelectedRValue { get => selectedRValue; set { selectedRValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRValue))); } }
        private GenericDeviceValue SelectedWValue { get => selectedWValue; set { selectedWValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedWValue))); } }

        public new event PropertyChangedEventHandler PropertyChanged;

        public GenericDeviceWindow(Block block)
        {
            this.block = block;
            //foreach(InputDefinition inputDefinition in blockDefinition.Inputs)
            //{
            //    WValues.Add()
            //}

            List<GenericDeviceValue> rValues = (List<GenericDeviceValue>)block.Parameters.Single(p => p.Definition.PropertyInfo == typeof(GenericDevice).GetProperty(nameof(GenericDevice.RValues))).Value;
            foreach(var v in rValues)
            {
                RValues.Add(v);
            }

            List<GenericDeviceValue> wValues = (List<GenericDeviceValue>)block.Parameters.Single(p => p.Definition.PropertyInfo == typeof(GenericDevice).GetProperty(nameof(GenericDevice.WValues))).Value;
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
            RValues.Add(new GenericDeviceValue());
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
            WValues.Add(new GenericDeviceValue());
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

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            BlockDefinition blockDefinition = new BlockDefinition() { Name = "dev" };
            foreach(var v in RValues)
            {
                blockDefinition.Outputs.Add(new OutputDefinition() { Name = v.Name, Type = v.Type, Disablable = false, DefaultDisabled = false });
            }
            foreach (var v in WValues)
            {
                blockDefinition.Inputs.Add(new InputDefinition() { Name = v.Name, Type = v.Type, Disablable = false, DefaultDisabled = false });
            }
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
