using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Views
{
    public class WireView : Polyline
    {
        public InputView InputView { get; }
        public OutputView OutputView { get; }
        public WireView(InputView inputView, OutputView outputView)
        {
            InputView = inputView;
            OutputView = outputView;

            Stroke = Brushes.Black;
            StrokeThickness = 1;
            StrokeLineCap = PenLineCap.Round;
            UpdatePoints();

            PointerEnter += WireView_PointerEnter;
            PointerLeave += WireView_PointerLeave;
        }

        private void WireView_PointerLeave(object sender, Avalonia.Input.PointerEventArgs e)
        {
            StrokeThickness = 1;
        }

        private void WireView_PointerEnter(object sender, Avalonia.Input.PointerEventArgs e)
        {
            StrokeThickness = 2;
        }

        public void UpdatePoints()
        {
            Points = new List<Point>();

            Point pO = OutputView.Position;
            Point pI = InputView.Position;

            Points.Add(pO);
            Points.Add(pI);
        }
    }
}
