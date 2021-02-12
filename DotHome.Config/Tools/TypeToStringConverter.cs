using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotHome.Config.Tools
{
    public class TypeToStringConverter : IValueConverter
    {
        public static TypeToStringConverter Instance { get; } = new TypeToStringConverter();

        private TypeToStringConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type t = (Type)value;
            if(t.IsGenericType)
            {
                return t.Name.Remove(t.Name.IndexOf('`')) + "<" + string.Join(", ", t.GetGenericArguments().Select(tt => tt.Name)) + ">";
            }
            else
            {
                return ((Type)value).Name;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
