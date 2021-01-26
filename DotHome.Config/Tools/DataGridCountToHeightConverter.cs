using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    class DataGridCountToHeightConverter : IValueConverter
    {
        public static DataGridCountToHeightConverter Instance { get; } = new DataGridCountToHeightConverter();

        private DataGridCountToHeightConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value + 1) * 25 + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
