using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class SelectedToZIndexConverter : IValueConverter
    {
        public static SelectedToZIndexConverter Instance { get; } = new SelectedToZIndexConverter();

        private static int counter = 0;

        private SelectedToZIndexConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            counter = (counter + 1) % int.MaxValue;
            return ((bool)value) ? int.MaxValue : (counter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
