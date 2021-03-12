using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    public abstract class TextCommunicationProvider : CommunicationProvider
    {
        protected abstract event Action<string> TextReceived;
        
        protected abstract void SendText(string text, TextDevice target);

    }
}
