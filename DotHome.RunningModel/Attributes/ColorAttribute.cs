using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    /// <summary>
    /// Defines color of the block in Config GUI. Give color in text (red) or hex representation (#FFFF0000)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ColorAttribute : Attribute
    {
        public Color Color { get; }
        public ColorAttribute(string color)
        {
            Color = ColorTranslator.FromHtml(color);
        }
    }
}
