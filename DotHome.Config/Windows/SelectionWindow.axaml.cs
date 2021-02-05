using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections;

namespace DotHome.Config.Windows
{
    public class SelectionWindow : Window
    {
        private IEnumerable Items { get; set; }

        private object SelectedItem { get; set; }

        private string Text { get; set; }

        public SelectionWindow(string text, IEnumerable items) : this()
        {
            Text = text;
            Items = items;
            DataContext = this;
        }

        public SelectionWindow()
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

        private void Ok_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(SelectedItem);
        }

        private void Cancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}
