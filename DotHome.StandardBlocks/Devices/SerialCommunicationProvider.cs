using DotHome.RunningModel;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class SerialCommunicationProvider : TextCommunicationProvider<SerialDevice>, IDisposable
    {
        private const int DEFAULT_BAUDRATE = 115200;

        protected override event Action<string, SerialDevice> TextReceived;
        private CancellationTokenSource cancellationTokenSource;
        private AsyncQueue<Tuple<string, SerialDevice>> outgoingQueue = new AsyncQueue<Tuple<string, SerialDevice>>();
        private List<Task> tasks = new List<Task>();
        private Dictionary<string, Tuple<SerialPort, StreamReader, StreamWriter>> ports = new Dictionary<string, Tuple<SerialPort, StreamReader, StreamWriter>>();

        public SerialCommunicationProvider()
        {
            cancellationTokenSource = new CancellationTokenSource();
            tasks.Add(SendingLoop().WithCancellation(cancellationTokenSource.Token));
        }

        protected override void SendText(string text, SerialDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
        }

        public override void RegisterDevice(SerialDevice device)
        {
            base.RegisterDevice(device);
            device.SerialPort = new SerialPort(device.PortName, device.Baudrate);
            device.SerialPort.Open();
            device.Reader = new StreamReader(device.SerialPort.BaseStream);
            device.Writer = new StreamWriter(device.SerialPort.BaseStream);
            tasks.Add(ReceivingLoop(device).WithCancellation(cancellationTokenSource.Token));
        }

        private async Task ReceivingLoop(SerialDevice device)
        {
            try
            {
                while (true)
                {
                    string line = await device.Reader.ReadLineAsync();
                    TextReceived?.Invoke(line, device);
                }
            }
            catch { }
        }

        private async Task SendingLoop()
        {
            try
            {
                while (true)
                {
                    var tuple = await outgoingQueue.DequeueAsync();
                    if (tuple != null)
                    {
                        if (tuple.Item2 == null)
                        {
                            string[] ports = SerialPort.GetPortNames();
                            foreach (var port in ports)
                            {
                                if (this.ports.TryGetValue(port, out var serialPort))
                                {
                                    await serialPort.Item3.WriteLineAsync(tuple.Item1);
                                }
                                else
                                {
                                    var serialPort2 = new SerialPort(port, DEFAULT_BAUDRATE);
                                    var streamReader = new StreamReader(serialPort2.BaseStream);
                                    var streamWriter = new StreamWriter(serialPort2.BaseStream) { NewLine = "\n", AutoFlush = true };
                                    this.ports[port] = new(serialPort2, streamReader, streamWriter);
                                    await streamWriter.WriteLineAsync(tuple.Item1);
                                }
                            }
                        }
                        else
                        {
                            if (((SerialDevice)tuple.Item2).Writer != null)
                            {
                                await ((SerialDevice)tuple.Item2).Writer?.WriteLineAsync(tuple.Item1);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            foreach (Task task in tasks)
            {
                try
                {
                    task.WaitWithoutInlining();
                }
                catch { }
            }
            foreach (var port in ports)
            {
                port.Value.Item2.Dispose();
                port.Value.Item3.Dispose();
                port.Value.Item1.Dispose();
            }
        }
    }
}
