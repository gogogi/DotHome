using Avalonia.Data.Converters;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config.Tools
{
    public class BlockNameConverter : IValueConverter
    {
        public static BlockNameConverter Instance { get; } = new BlockNameConverter();
        private BlockNameConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProgrammingModel.Block block = (ProgrammingModel.Block)value;
            return block.Parameters.Single(p => p.Definition.Name == nameof(NamedBlock.Name)).ValueAsString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
