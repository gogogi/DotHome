using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DotHome.Config.Tools;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Views
{
    public abstract class ABlockView : UserControl
    {
        private static Cursor cursor = new Cursor(StandardCursorType.SizeAll);

        public ABlock Block => (ABlock)DataContext;
                        
        public new double Width => Bounds.Width;
        public new double Height => Bounds.Height;

        public virtual InputView[] Inputs { get; } = new InputView[0];
        public virtual OutputView[] Outputs { get; } = new OutputView[0];

        public ABlockView()
        {
            Cursor = cursor;
            Padding = new Thickness(2);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Parent.Bind(Canvas.LeftProperty, new Binding("X"));
            Parent.Bind(Canvas.TopProperty, new Binding("Y"));
            Parent.Bind(ZIndexProperty, new Binding("Selected") { Converter = SelectedToZIndexConverter.Instance });
            this.Bind(BackgroundProperty, new Binding("Selected") { Converter = SelectedToBackgroundConverter.Instance });
        }
    }
}
