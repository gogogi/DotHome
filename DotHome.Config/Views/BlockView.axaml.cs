using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DotHome.Config.Views
{
    public class BlockView : ABlockView
    {
        private TextBlock textBlockName;
        private StackPanel stackPanelInputs;
        private StackPanel stackPanelOutputs;

        private InputView[] inputs;
        private OutputView[] outputs;

        public override InputView[] Inputs => inputs;
        public override OutputView[] Outputs => outputs;

        public BlockDefinition BlockDefinition { get; }
        public BlockView(BlockDefinition blockDefinition) : this()
        {
            BlockDefinition = blockDefinition;

            textBlockName = this.FindControl<TextBlock>("textBlockName");
            stackPanelInputs = this.FindControl<StackPanel>("stackPanelInputs");
            stackPanelOutputs = this.FindControl<StackPanel>("stackPanelOutputs");

            Debug.WriteLine(blockDefinition.Name);

            textBlockName.Text = blockDefinition.Name;

            foreach(var input in blockDefinition.Inputs)
            {
                stackPanelInputs.Children.Add(new InputView(input));
            }

            foreach (var output in blockDefinition.Outputs)
            {
                stackPanelOutputs.Children.Add(new OutputView(output));
            }

            inputs = stackPanelInputs.Children.Cast<InputView>().ToArray();
            outputs = stackPanelOutputs.Children.Cast<OutputView>().ToArray();

            Debug.WriteLine(inputs.Length);
        }
        public BlockView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
