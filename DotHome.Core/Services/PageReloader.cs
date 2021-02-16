using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public class PageReloader
    {
        public event Action ReloadForced;

        public void ForceReload()
        {
            ReloadForced?.Invoke();
        }
    }
}
