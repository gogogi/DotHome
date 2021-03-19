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
    public class TcpCommunicationProvider : TextCommunicationProvider<TcpDevice>, IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;
        private TcpListener tcpListener;
        private int port;
        private byte[] receivingBuffer = new byte[1000];
        private AsyncQueue<Tuple<string, GenericDevice>> outgoingQueue = new AsyncQueue<Tuple<string, GenericDevice>>();
        private List<Task> tasks = new List<Task>();
        private List<Tuple<TcpClient, StreamReader, StreamWriter>> clients = new List<Tuple<TcpClient, StreamReader, StreamWriter>>();

        protected override event Action<string, GenericDevice> TextReceived;

        public TcpCommunicationProvider(IConfiguration configuration)
        {
            port = int.Parse(configuration["TcpPort"]);
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
            foreach (var client in clients)
            {
                client.Item2.Dispose();
                client.Item3.Dispose();
                client.Item1.Dispose();
            }
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
                Debug.WriteLine("Connected " + address + " " + devices.Count);
                Debug.WriteLine((TcpDevice)devices.SingleOrDefault(d => ((TcpDevice)d).IP == address.MapToIPv4().ToString()) == null);
                TcpDevice device = (TcpDevice)devices.SingleOrDefault(d => ((TcpDevice)d).IP == address.MapToIPv4().ToString()) ?? new TcpDevice(this) { IPAddress = address, IP = address.MapToIPv4().ToString() };
                if(device.Client != null) device.Client.Dispose();
                device.Client = client;
                device.Reader = new StreamReader(client.GetStream());
                device.Writer = new StreamWriter(client.GetStream());

                clients.Add(new(client, device.Reader, device.Writer));

                tasks.Add(Task.Factory.StartNew(() => ReceivingLoop(cancellationTokenSource.Token, device), TaskCreationOptions.LongRunning));
            }
        }

        private void ReceivingLoop(CancellationToken cancellationToken, TcpDevice device)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    string line = device.Reader.ReadLine();
                    if (line != null)
                    {
                        TextReceived?.Invoke(line, device);
                    }
                }
                catch(IOException)
                {
                    break;
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
                        foreach(var client in clients)
                        {
                            client.Item3.Write(tuple.Item1 + "\n");
                            client.Item3.Flush();
                        }
                    }
                    else
                    {
                        ((TcpDevice)tuple.Item2).Writer?.Write(tuple.Item1 + "\n");
                        ((TcpDevice)tuple.Item2).Writer?.Flush();
                    }
                }
            }
        }
    }
}
