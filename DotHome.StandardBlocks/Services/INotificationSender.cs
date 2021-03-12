using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Services
{
    public interface INotificationSender
    {
        public void SendNotification(string message, string url, AuthenticatedBlock source);
    }
}
