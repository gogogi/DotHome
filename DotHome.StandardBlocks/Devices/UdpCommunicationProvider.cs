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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class UdpCommunicationProvider : TextCommunicationProvider<UdpDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private UdpClient udpClient;
        private int port;
        private byte[] receivingBuffer = new byte[1000];
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();
        private List<Task> tasks = new List<Task>();

        protected override event Action<string, GenericDevice> TextReceived;

        public UdpCommunicationProvider(IConfiguration configuration)
        {
            port = int.Parse(configuration["UdpPort"]);
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            cancellationTokenSource = new CancellationTokenSource();
            tasks.Add(ReceivingLoop().WithCancellation(cancellationTokenSource.Token));
            tasks.Add(SendingLoop().WithCancellation(cancellationTokenSource.Token));
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            foreach(Task task in tasks)
            {
                try
                {
                    task.WaitWithoutInlining();
                }
                catch { }
            }
            udpClient.Dispose();
        }

        protected override void SendText(string text, GenericDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
            Debug.WriteLine("enqueue " + text);
        }

        private async Task ReceivingLoop()
        {
            try
            {
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    string text = Encoding.Default.GetString(result.Buffer);
                    var device = devices.SingleOrDefault(d => ((UdpDevice)d).IPAddress.Equals(result.RemoteEndPoint.Address)) ?? new UdpDevice(this) { IPAddress = result.RemoteEndPoint.Address.MapToIPv4(), IP = result.RemoteEndPoint.Address.MapToIPv4().ToString() };
                    TextReceived?.Invoke(text, device);
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
                        byte[] buffer = Encoding.Default.GetBytes(tuple.Item1);
                        await udpClient.SendAsync(buffer, buffer.Length, new IPEndPoint((tuple.Item2 as UdpDevice)?.IPAddress ?? IPAddress.Broadcast, port));
                    }
                }
            }
            catch { }
        }
    }
}
