using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DotHome.Config.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Views
{
    public abstract class ABlockView : UserControl
    {
        private static Cursor cursor = new Cursor(StandardCursorType.SizeAll);

        private double x, y;

        public int Id { get; set; }

        public bool Selected
        {
            get => Background == Brushes.Black;
            set
            {
                Background = value ? Brushes.Black : Brushes.Gray;
                Command.ForceChanges();
                ZIndex = value ? 2 : 1;
            }
        }

        public double X
        {
            get => x;
            set
            {
                x = value;
                Canvas.SetLeft(this, Math.Round(value));
            }
        }
        public double Y
        {
            get => y;
            set
            {
                y = value;
                Canvas.SetTop(this, Math.Round(value));
            }
        }

        public new double Width => Bounds.Width;
        public new double Height => Bounds.Height;

        public virtual InputView[] Inputs { get; } = new InputView[0];
        public virtual OutputView[] Outputs { get; } = new OutputView[0];

        public ABlockView()
        {
            Cursor = cursor;
        }
    }
}
