using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.RunningModel
{
    public abstract class AVisualBlock : ANamedBlock
    {
        internal event Action VisualStateChanged;

        [BlockParameter]
        public List<User> AllowedUsers { get; } = new List<User>();

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
