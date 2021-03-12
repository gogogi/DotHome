using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.RunningModel.Devices
{
    public class UDPDevice : TextDevice
    {
        public IPAddress IPAddress { get; set; }

        [Parameter(true), RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"), Unique]
        public string IP { get; set; } = "127.0.0.1";

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void ReadValues()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override void WriteValues()
        {
            throw new NotImplementedException();
        }
    }
}
