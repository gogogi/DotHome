using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Notifications
{
    [Description("Sends notification on changed input"), Category("Notifications")]
    public class Alert : ABlock
    {
        [Description("Input")]
        public Input<bool> I { get; set; }

        [BlockParameter(true), Description("The message to show")]
        public string Message { get; set; } = "Message";

        [BlockParameter, Description("If true, notification is generated on falling edge")]
        public bool FallingEdge { get; set; } = false;

        public override void Init()
        {
            
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
