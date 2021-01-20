using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    class BlockColorConverter : IValueConverter
    {
        public static BlockColorConverter Instance { get; } = new BlockColorConverter();

        private BlockColorConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            return new SolidColorBrush(new Color(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
