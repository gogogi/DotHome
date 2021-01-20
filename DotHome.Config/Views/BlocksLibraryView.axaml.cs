using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;

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
