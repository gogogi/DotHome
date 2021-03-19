using DotHome.RunningModel;
using DotHome.RunningModel.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class UdpCommunicationProvider : TextCommunicationProvider<UdpDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private Socket socket;
        private int port;
        private byte[] receivingBuffer = new byte[1000];
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();

        protected override event Action<string, GenericDevice> TextReceived;

        public UdpCommunicationProvider(IConfiguration configuration)
        {
            port = int.Parse(configuration["UdpPort"]);
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ReceivingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => SendingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }

        protected override void SendText(string text, GenericDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
            Debug.WriteLine("enqueue " + text);
        }

        private void ReceivingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                EndPoint endPoint = new IPEndPoint(0, 0);
                int count = socket.ReceiveFrom(receivingBuffer, ref endPoint);
                string text = Encoding.Default.GetString(receivingBuffer, 0, count);
                IPAddress address = ((IPEndPoint)endPoint).Address;
                var device = devices.SingleOrDefault(d => ((UdpDevice)d).IPAddress == address) ?? new UdpDevice(this) { IPAddress = address, IP = address.MapToIPv4().ToString() };
                TextReceived?.Invoke(text, device);
            }
        }

        private async void SendingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tuple = await outgoingQueue.DequeueAsync(cancellationToken);
                if (tuple != null)
                {
                    socket.SendTo(Encoding.Default.GetBytes(tuple.Item1), new IPEndPoint((tuple.Item2 as UdpDevice)?.IPAddress ?? IPAddress.Broadcast, port));
                }
            }
        }
    }
}
