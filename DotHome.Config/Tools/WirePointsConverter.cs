using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Data.Converters;
using Avalonia.Media;
using DotHome.Config.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    class WirePointsConverter : IMultiValueConverter
    {
        //private static readonly double ratio = 4;

        public static WirePointsConverter Instance { get; } = new WirePointsConverter();

        private WirePointsConverter() { }

        public object Convert(IList<object> values, Type targetType, dynamic parameter, CultureInfo culture)
        {
            InputView inputView = parameter.InputView;
            OutputView outputView = parameter.OutputView;

            //Point po = outputView.Position;
            //Point pi = inputView.Position;

            //var list = new List<Point>() { po };

            //if ((pi.X - po.X) * ratio > Math.Abs(pi.Y - po.Y))
            //{
            //    var d = Math.Abs(po.Y - pi.Y) / ratio;
            //    if (d < pi.X - po.X)
            //    {
            //        list.Add(po + new Point((pi.X - po.X - d) / 2, 0));
            //        list.Add(pi - new Point((pi.X - po.X - d) / 2, 0));
            //        list.Add(pi);
            //        return list;
            //    }
            //}
            //else
            //{
            //    var d = (po.X - pi.X) + Math.Abs(pi.Y - po.Y) / ratio;
            //    var dy = Math.Abs(pi.Y - po.Y) / 2;
            //    var dx = dy / ratio;

            //    list.Add(po + new Point(dx, po.Y > pi.Y ? -dy : dy));
            //    list.Add(pi + new Point(-dx, po.Y > pi.Y ? dy : -dy));

            //    list.Add(pi);
            //    return list;
            //}

            return new List<Point>() { inputView.Position, outputView.Position };
        }
    }
}
