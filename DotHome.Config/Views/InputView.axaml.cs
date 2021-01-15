using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.Config.Tools;
using System.Diagnostics;

namespace DotHome.Config.Views
{
    public class InputView : UserControl
    {
        private TextBlock textBlockName;
        private Polygon polygon;

        public bool Disabled { get => !IsVisible; set => IsVisible = !value; }

        public Point Position => polygon.TranslatePoint(new Point(0, 4), this.ParentOfType<Canvas>()) ?? new Point(0, 0);

        public InputView(InputDefinition inputDefinition) : this()
        {
            textBlockName.Text = inputDefinition.Name;
            Disabled = inputDefinition.DefaultDisabled;
        }

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
