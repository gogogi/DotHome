using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config.Tools
{
    public class DebugValueConverter : IValueConverter
    {
        public static DebugValueConverter Instance { get; } = new DebugValueConverter();
        private DebugValueConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double d)
            {
                return d.ToString("0.00");
            }
            else if(value == null)
            {
                return "";
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
