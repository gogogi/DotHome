using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DotHome.StandardBlocks.Services
{
    public class TimeProvider : IBlockService
    {
        public DateTime CurrentTime { get; private set; }
        public long Millis { get; private set; }

        public void Init()
        {
            CurrentTime = DateTime.Now;
            Millis =  CurrentTime.Ticks / (TimeSpan.TicksPerSecond / 1000);
        }

        public void Run()
        {
            CurrentTime = DateTime.Now;
            Millis = CurrentTime.Ticks / (TimeSpan.TicksPerSecond / 1000);
        }
    }
}
