using DotHome.RunningModel;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.StandardBlocks.Devices
{
    public class TcpCommunicationProvider : TextCommunicationProvider<TcpDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private Socket udpSocket;
        private TcpListener tcpListener;
        private int port;
        private byte[] receivingBuffer = new byte[1000];
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();
        private List<Task> tasks = new List<Task>();
        private List<TcpClient> clients = new List<TcpClient>();
        private List<StreamReader> readers = new List<StreamReader>();
        private List<StreamWriter> writers = new List<StreamWriter>();

        protected override event Action<string, GenericDevice> TextReceived;

        public TcpCommunicationProvider(IConfiguration configuration)
        {
            port = int.Parse(configuration["TcpPort"]);
            udpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
            tcpListener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            tasks.Add(Task.Factory.StartNew(() => AcceptingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning));
            tasks.Add(Task.Factory.StartNew(() => SendingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning));
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            foreach(Task task in tasks)
            {
                task.Wait();
            }
            foreach (var reader in readers) reader.Dispose();
            foreach (var writer in writers) writer.Dispose();
            foreach (var client in clients) client.Dispose();
        }

        protected override void SendText(string text, GenericDevice target)
        {
            outgoingQueue.Enqueue(new(text, target));
        }

        private void AcceptingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = tcpListener.AcceptTcpClient();
                var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                TcpDevice device = (TcpDevice)devices.SingleOrDefault(d => ((TcpDevice)d).IPAddress == address) ?? new TcpDevice(this) { IPAddress = address, IP = address.MapToIPv4().ToString() };
                if(device.Client != null)
                {
                    device.Client.Dispose();
                    device.Client = client;
                    device.Reader = new StreamReader(client.GetStream());
                    device.Writer = new StreamWriter(client.GetStream());

                    clients.Add(client);
                    readers.Add(device.Reader);
                    writers.Add(device.Writer);

                    tasks.Add(Task.Factory.StartNew(() => ReceivingLoop(cancellationTokenSource.Token, device), TaskCreationOptions.LongRunning));
                }
            }
        }

        private void ReceivingLoop(CancellationToken cancellationToken, TcpDevice device)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string line = device.Reader.ReadLine();
                if (line != null)
                {
                    TextReceived?.Invoke(line, device);
                }
            }
        }

        private async void SendingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tuple = await outgoingQueue.DequeueAsync(cancellationToken);
                if (tuple != null)
                {
                    if(tuple.Item2 == null)
                    {
                        udpSocket.SendTo(Encoding.Default.GetBytes(tuple.Item1), new IPEndPoint(IPAddress.Broadcast, port));
                    }
                    else
                    {
                        ((TcpDevice)tuple.Item2).Writer.Write(tuple.Item1 + "\n");
                    }
                }
            }
        }
    }
}
