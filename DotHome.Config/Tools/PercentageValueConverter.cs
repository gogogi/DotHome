using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config.Tools
{
    public class PercentageValueConverter : IValueConverter
    {
        public static PercentageValueConverter Instance { get; } = new PercentageValueConverter();

        private PercentageValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0:0.00}%", (double)value * 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
