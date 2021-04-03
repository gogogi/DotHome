using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class SerialDevice : GenericDevice
    {
        [Parameter(true)]
        public string PortName { get; set; }

        [Parameter(false), Range(9600, 115200)]
        public int Baudrate { get; set; }

        public SerialPort SerialPort { get; set; }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        public SerialDevice(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }
    }
}
