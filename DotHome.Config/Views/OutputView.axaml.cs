using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;

namespace DotHome.Config.Views
{
    public class OutputView : UserControl
    {
        public Value Output => (Value)DataContext;

        private TextBlock textBlockName;
        private Polygon polygon;

        public Point Position => polygon.TranslatePoint(new Point(10, 4), this.ParentOfType<Canvas>()) ?? new Point(0, 0);

        public OutputView()
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
