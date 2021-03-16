using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.ProgrammingModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace DotHome.Config.Windows
{
    public class DeviceSelectionWindow : Window, INotifyPropertyChanged
    {
        private Block selectedBlock;

        private List<Block> Blocks { get; set; }

        private Block SelectedBlock { get => selectedBlock; set { selectedBlock = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBlock))); } }

        public new event PropertyChangedEventHandler PropertyChanged;

        public DeviceSelectionWindow(List<Block> blocks) : this()
        {
            Blocks = blocks;
            DataContext = this;
        }

        public DeviceSelectionWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(SelectedBlock);
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
