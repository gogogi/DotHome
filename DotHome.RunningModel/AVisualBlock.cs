using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class AVisualBlock : ANamedBlock
    {
        internal event Action VisualStateChanged;

        protected void VisualStateHasChanged()
        {
            VisualStateChanged?.Invoke();
        }
    }
}
