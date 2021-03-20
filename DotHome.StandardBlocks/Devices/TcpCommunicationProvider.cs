using DotHome.RunningModel;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class TcpCommunicationProvider : TextCommunicationProvider<TcpDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private TcpListener tcpListener;
        private int port;
        private bool stop;
        private byte[] receivingBuffer = new byte[1000];
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();
        private List<Task> tasks = new List<Task>();
        private Dictionary<IPAddress, Tuple<TcpClient, StreamReader, StreamWriter>> clients = new Dictionary<IPAddress, Tuple<TcpClient, StreamReader, StreamWriter>>();

        protected override event Action<string, GenericDevice> TextReceived;

        
        public TcpCommunicationProvider(IConfiguration configuration)
        {
            cancellationTokenSource = new CancellationTokenSource();
            port = int.Parse(configuration["TcpPort"]);
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptCallback, null);
            tasks.Add(SendingLoop().WithCancellation(cancellationTokenSource.Token));
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if(!stop)
            {
                Debug.WriteLine("called");
                var client = tcpListener.EndAcceptTcpClient(ar);
                var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                tcpListener.BeginAcceptTcpClient(AcceptCallback, null);
                TcpDevice device = (TcpDevice)devices.SingleOrDefault(d => ((TcpDevice)d).IPAddress.Equals(address)) ?? new TcpDevice(this) { IPAddress = address, IP = address.MapToIPv4().ToString() };
                if(clients.TryGetValue(address, out var tuple))
                {
                    tuple.Item2.Dispose();
                    tuple.Item3.Dispose();
                    tuple.Item1.Dispose();
                }
                device.Client = client;
                device.Reader = new StreamReader(client.GetStream());
                device.Writer = new StreamWriter(client.GetStream());

                clients[address] = new (client, device.Reader, device.Writer);
                tasks.Add(ReceivingLoop(device).WithCancellation(cancellationTokenSource.Token));
            }
        }

        public void Dispose()
        {
            stop = true;
            cancellationTokenSource.Cancel();
            foreach(Task task in tasks)
            {
                try
                {
                    task.WaitWithoutInlining();
                }
                catch { }
            }
            
            tcpListener.Stop();
            foreach (var client in clients)
            {
                client.Value.Item2.Dispose();
                client.Value.Item3.Dispose();
                client.Value.Item1.Dispose();
            }
        }

        protected override void SendText(string text, GenericDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
        }

        private async Task ReceivingLoop(TcpDevice device)
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
                            foreach (var client in clients)
                            {
                                await client.Value.Item3.WriteAsync(tuple.Item1 + "\n");
                                await client.Value.Item3.FlushAsync();
                            }
                        }
                        else
                        {
                            if(((TcpDevice)tuple.Item2).Writer != null) {
                                await ((TcpDevice)tuple.Item2).Writer?.WriteAsync(tuple.Item1 + "\n");
                                await ((TcpDevice)tuple.Item2).Writer?.FlushAsync();
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
