using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    /// <summary>
    /// A <see cref="AuthenticatedBlock"/> that is actually displayed in visualisation GUI (Switch, Button, Thermostat...)
    /// </summary>
    public abstract class VisualBlock : AuthenticatedBlock
    {
        /// <summary>
        /// Acts to infom <see cref="VisualBlockComponent{T}"/> that it should re-render itself
        /// </summary>
        internal event Action VisualStateChanged;
        
        [Parameter(true)]
        public Room Room { get; set; }

        [Parameter(true)]
        public Category Category { get; set; }

        /// <summary>
        /// To be called from <see cref="Block.Run"/> when the GUI should re-render itself
        /// </summary>
        protected void VisualStateHasChanged()
        {
            VisualStateChanged?.Invoke();
        }
    }
}
