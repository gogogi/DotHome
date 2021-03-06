﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.Config.Tools;
using System.Diagnostics;
using DotHome.ProgrammingModel;

namespace DotHome.Config.Views
{
    public class InputView : StackPanel
    {
        public Input Input => (Input)DataContext;

        private TextBlock textBlockName;
        private Polygon polygon;

        public bool Disabled { get => !IsVisible; set => IsVisible = !value; }

        public Point Position => polygon.TranslatePoint(new Point(0, 4), this.ParentOfType<Canvas>()) ?? new Point(990, 990);

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
