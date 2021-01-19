using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DotHome.Config.Tools;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DotHome.Config.Views
{
    public class WireView : Polyline
    {
        private static TranslateTransform t0 = new TranslateTransform(0, 0);
        private static TranslateTransform t1 = new TranslateTransform(0.001, 0.001);

        public Wire Wire => (Wire)DataContext;
        public WireView()
        {
            Stroke = Brushes.Black;
            StrokeThickness = 1;
            StrokeLineCap = PenLineCap.Round;

            PropertyChanged += WireView_PropertyChanged;

            PointerEnter += WireView_PointerEnter;
            PointerLeave += WireView_PointerLeave;
            LayoutUpdated += WireView_LayoutUpdated;
        }

        private void WireView_LayoutUpdated(object sender, EventArgs e)
        {
            RenderTransform = t0;
        }

        private void WireView_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Property == PointsProperty)
            {
                RenderTransform = t1;
            }
        }

        private void WireView_PointerLeave(object sender, Avalonia.Input.PointerEventArgs e)
        {
            StrokeThickness = 1;
        }

        private void WireView_PointerEnter(object sender, Avalonia.Input.PointerEventArgs e)
        {
            StrokeThickness = 2;
        }
    }
}
