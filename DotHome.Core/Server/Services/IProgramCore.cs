using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Server.Services
{
    public interface IProgramCore
    {
        public void Start();

        public void Stop();

        public void StartDebugging();

        public void StopDebugging();

    }
}
