using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace DotHome.Config.Views
{
    public class ConstView : ABlockView
    {
        private TextBlock textBlock;
        private OutputView outputView;

        private object value;

        public Type Type { get; set; }

        public object Value { get => value; set { this.value = value; textBlock.Text = value.ToString(); } }

        public override OutputView[] Outputs => new OutputView[] { outputView };

        public ConstView()
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
