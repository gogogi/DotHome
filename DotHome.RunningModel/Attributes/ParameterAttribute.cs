using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel.Attributes
{
    /// <summary>
    /// Marks property of <see cref="Block"/> as configuration parameter - it is shown and can be edited in Config GUI
    /// </summary>
    public class ParameterAttribute : Attribute
    {
        public bool ShowInBlock { get; }

        /// <summary>
        /// </summary>
        /// <param name="showInBlock">Determines if the parameter should be showed inside the block itself or only in properties window</param>
        public ParameterAttribute(bool showInBlock = false)
        {
            ShowInBlock = showInBlock;
        }
    }
}
