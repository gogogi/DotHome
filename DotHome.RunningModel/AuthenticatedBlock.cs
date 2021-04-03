using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    /// <summary>
    /// A <see cref="NamedBlock"/> with access granted only to some <see cref="User"/>s
    /// </summary>
    public abstract class AuthenticatedBlock : NamedBlock
    {
        [Parameter]
        public List<User> AllowedUsers { get; set; } = new List<User>();
    }
}
