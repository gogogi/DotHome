using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel
{
    public abstract class AAuthenticatedBlock : ANamedBlock
    {
        [BlockParameter]
        public List<User> AllowedUsers { get; set; } = new List<User>();
    }
}
