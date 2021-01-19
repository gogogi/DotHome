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
        public static WirePointsConverter Instance { get; } = new WirePointsConverter();

        private static Random random = new Random();

        private WirePointsConverter() { }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            InputView inputView = ((dynamic)parameter).InputView;
            OutputView outputView = ((dynamic)parameter).OutputView;

            return new List<Point>() { inputView.Position, outputView.Position };
        }
    }
}
