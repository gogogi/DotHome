using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class TcpDevice : GenericDevice
    {
        public IPAddress IPAddress { get; set; }

        public TcpClient Client { get; set; }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        [Parameter(true), RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"), Unique]
        public string IP { get; set; } = "127.0.0.1";

        public TcpDevice(TcpCommunicationProvider communicationProvider) : base(communicationProvider)
        {

        }

        public override void Init()
        {
            IPAddress = IPAddress.Parse(IP);
        }
    }
}
