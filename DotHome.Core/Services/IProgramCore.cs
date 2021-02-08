using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public interface IProgramCore
    {
        public void Restart();

        public void StartDebugging();

        public void StopDebugging();

        public void Pause();

        public void Continue();

        public void Step();

        public List<AVisualBlock> VisualBlocks { get; }

        public double AverageCpuUsage { get; }
        public double MaxCpuUsage { get; }

    }
}