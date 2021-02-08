using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    public class BlockParameterAttribute : Attribute
    {
        public bool ShowInBlock { get; }
        public BlockParameterAttribute(bool showInBlock = false)
        {
            ShowInBlock = showInBlock;
        }
    }
}
