using Avalonia.Data.Converters;
using DotHome.Definitions;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockDefinitionToBlockConverter : IValueConverter
    {
        public static BlockDefinitionToBlockConverter Instance { get; } = new BlockDefinitionToBlockConverter();

        private BlockDefinitionToBlockConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Block((BlockDefinition)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
