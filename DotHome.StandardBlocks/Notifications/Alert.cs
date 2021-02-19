using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Notifications
{
    [Description("Sends notification on changed input"), Category("Notifications")]
    public class Alert : AAuthenticatedBlock
    {
        [Description("Input")]
        public Input<bool> I { get; set; }

        [BlockParameter, Description("If true, notification is generated on falling edge")]
        public bool FallingEdge { get; set; } = false;

        private INotificationSender notificationSender;

        private bool lastVal;

        public Alert(INotificationSender notificationSender)
        {
            this.notificationSender = notificationSender;
        }

        public override void Init()
        {
            
        }

        public override void Run()
        {
            if(lastVal != I.Val)
            {
                lastVal = I.Val;
                if(lastVal != FallingEdge)
                {
                    notificationSender.SendNotification(Name, "/", this);
                }
            }
        }
    }
}
