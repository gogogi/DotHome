using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config.Tools
{
    class NoSizeDecorator : Decorator
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Child.Measure(Size.Infinity);
            Child.ClipToBounds = false;
            return new Size(0, 0);
        }
    }
}
