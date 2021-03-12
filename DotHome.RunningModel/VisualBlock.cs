using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class VisualBlock : AuthenticatedBlock
    {
        internal event Action VisualStateChanged;
        
        [Parameter(true)]
        public Room Room { get; set; }

        [Parameter(true)]
        public Category Category { get; set; }

        protected void VisualStateHasChanged()
        {
            VisualStateChanged?.Invoke();
        }
    }
}
