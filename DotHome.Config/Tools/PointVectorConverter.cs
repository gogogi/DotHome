using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class PointVectorConverter : IValueConverter
    {
        public static PointVectorConverter Instance { get; } = new PointVectorConverter();

        private PointVectorConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (System.Drawing.Point)value;
            return new Vector(p.X, p.Y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (Vector)value;
            return new Point(v.X, v.Y);
        }
    }
}
