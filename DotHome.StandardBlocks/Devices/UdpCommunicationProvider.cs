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
    public class UDPCommunicationProvider : TextCommunicationProvider<UDPDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private Socket socket;
        private int port;
        private byte[] receivingBuffer = new byte[1000];
        private List<UDPDevice> searchDevices;
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();

        protected override event Action<string> TextReceived;
        private event Action<string, IPAddress> InternalTextReceived;

        public UDPCommunicationProvider(IConfiguration configuration)
        {
            InternalTextReceived += (text, address) => TextReceived?.Invoke(text);
            port = int.Parse(configuration["UdpPort"]);
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => ReceivingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(() => SendingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public override List<GenericDevice> SearchDevices()
        {
            searchDevices = new List<UDPDevice>();
            InternalTextReceived += Search_TextReceived;
            SendText("Details", null);
            Thread.Sleep(5000);
            InternalTextReceived -= Search_TextReceived;
            return searchDevices.Cast<GenericDevice>().ToList();
        }

        private void Search_TextReceived(string msg, IPAddress sender)
        {
            if (msg.StartsWith("DetailsResponse"))
            {
                try
                {
                    UDPDevice device = new UDPDevice(this) { IPAddress = sender, IP = sender.MapToIPv4().ToString() };
                    GenericDeviceParser.Parse(msg.Substring("DetailsResponse ".Length), device);
                    searchDevices.Add(device);
                }
                catch
                {

                }
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }

        protected override void SendText(string text, GenericDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
        }

        private void ReceivingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                EndPoint endPoint = new IPEndPoint(0, 0);
                int count = socket.ReceiveFrom(receivingBuffer, ref endPoint);
                string text = Encoding.Default.GetString(receivingBuffer, 0, count);
                InternalTextReceived?.Invoke(text, ((IPEndPoint)endPoint).Address);
            }
        }

        private async void SendingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tuple = await outgoingQueue.DequeueAsync(cancellationToken);
                if (tuple != null)
                {
                    Debug.WriteLine(tuple.Item1);
                    socket.SendTo(Encoding.Default.GetBytes(tuple.Item1), new IPEndPoint((tuple.Item2 as UDPDevice)?.IPAddress ?? IPAddress.Broadcast, port));
                }
            }
        }
    }
}
