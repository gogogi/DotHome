using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace DotHome.Config.Views
{
    public class RefSinkView : ABlockView
    {
        private TextBlock textBlock;
        private InputView inputView;

        public string Reference { get => textBlock.Text; set => textBlock.Text = value; }

        public override InputView[] Inputs => new InputView[] { inputView };

        public RefSinkView()
        {
            this.InitializeComponent();

            textBlock = this.FindControl<TextBlock>("textBlock");
            inputView = this.FindControl<InputView>("inputView");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
