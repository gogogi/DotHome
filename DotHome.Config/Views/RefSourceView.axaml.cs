using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotHome.Config.Views
{
    public class RefSourceView : ABlockView
    {
        private TextBlock textBlock;
        private OutputView outputView;

        public string Reference { get => textBlock.Text; set => textBlock.Text = value; }

        public override OutputView[] Outputs => new OutputView[] { outputView };
        public RefSourceView()
        {
            this.InitializeComponent();

            textBlock = this.FindControl<TextBlock>("textBlock");
            outputView = this.FindControl<OutputView>("outputView");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
