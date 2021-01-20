using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    public class ColorAttribute : Attribute
    {
        public Color Color { get; }
        public ColorAttribute(string color)
        {
            Color = Color.FromName(color);
        }
    }
}
