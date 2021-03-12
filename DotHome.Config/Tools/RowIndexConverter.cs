using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config.Tools
{
    public class RowIndexConverter : IMultiValueConverter
    {
        public static RowIndexConverter Instance { get; } = new RowIndexConverter();

        private RowIndexConverter() { }

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IList)values[0]).IndexOf(values[1]).ToString();
        }
    }
}
