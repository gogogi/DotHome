using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotHome.Config.Views
{
    public class BlocksLibraryView : UserControl
    {
        public BlocksLibraryView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
