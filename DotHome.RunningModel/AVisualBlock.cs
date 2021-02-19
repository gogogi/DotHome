using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class AVisualBlock : AAuthenticatedBlock
    {
        internal event Action VisualStateChanged;
        
        [BlockParameter(true)]
        public Room Room { get; set; }

        [BlockParameter(true)]
        public Category Category { get; set; }

        protected void VisualStateHasChanged()
        {
            VisualStateChanged?.Invoke();
        }
    }
}
