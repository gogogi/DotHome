using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public bool ShowInBlock { get; }
        public ParameterAttribute(bool showInBlock = false)
        {
            ShowInBlock = showInBlock;
        }
    }
}
