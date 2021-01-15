using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.Config.Tools;

namespace DotHome.Config.Views
{
    public class OutputView : UserControl
    {
        private TextBlock textBlockName;
        private Polygon polygon;

        public bool Disabled { get => !IsVisible; set => IsVisible = !value; }

        public Point Position => polygon.TranslatePoint(new Point(8, 4), this.ParentOfType<Canvas>()) ?? new Point(0, 0);

        public OutputView(OutputDefinition outputDefinition) : this()
        {
            textBlockName.Text = outputDefinition.Name;
            Disabled = outputDefinition.DefaultDisabled;
        }

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
