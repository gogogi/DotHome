using DotHome.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Model
{
    public abstract class VisualBlock : NamedBlock
    {
        internal event Action VisualStateChanged;

        [BlockParameter]
        public List<User> AllowedUsers { get; } = new List<User>();

        [BlockParameter(true)]
        public Room Room { get; set; }

        [BlockParameter]
        public Category Category { get; set; }

        protected void VisualStateHasChanged()
        {
            VisualStateChanged?.Invoke();
        }
    }
}
