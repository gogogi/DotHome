using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotHome.Config.Views
{
    public class SelectionPropertiesView : UserControl
    {
        public SelectionPropertiesView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
