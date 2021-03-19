using DotHome.RunningModel.Attributes;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class UdpDevice : GenericDevice
    {
        public IPAddress IPAddress { get; set; }

        [Parameter(true), RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"), Unique]
        public string IP { get; set; } = "127.0.0.1";

        public UdpDevice(UdpCommunicationProvider communicationProvider) : base(communicationProvider)
        {

        }

        public override void Init()
        {
            IPAddress = IPAddress.Parse(IP);
            base.Init();
        }
    }
}
