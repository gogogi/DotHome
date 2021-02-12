using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using System.Diagnostics;

namespace DotHome.Config.Views
{
    public class InputView : StackPanel
    {
        public Value Input => (Value)DataContext;

        private TextBlock textBlockName;
        private Polygon polygon;

        public Point Position => polygon.TranslatePoint(new Point(0, 4), this.ParentOfType<Canvas>()) ?? new Point(0, 0);

        public InputView()
        {
            this.InitializeComponent();

            textBlockName = this.FindControl<TextBlock>("textBlockName");
            polygon = this.FindControl<Polygon>("polygon");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
