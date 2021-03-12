using DotHome.RunningModel.Tools;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

namespace DotHome.RunningModel.Devices
{
    //public class UDPCommunicationProvider : TextCommunicationProvider, IDisposable
    //{
    //    private CancellationTokenSource cancellationTokenSource;
    //    private Socket socket;
    //    private int port;
    //    private byte[] receivingBuffer = new byte[1000];
    //    private List<GenericDevice> searchDevices;

    //    protected override event Action<string> TextReceived;
    //    private event Action<string, IPAddress> InternalTextReceived;

    //    public UDPCommunicationProvider(IConfiguration configuration)
    //    {
    //        InternalTextReceived += (text, address) => TextReceived?.Invoke(text);
    //        port = int.Parse(configuration["UdpPort"]);
    //        socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
    //        socket.Bind(new IPEndPoint(IPAddress.Parse(configuration["IPAddress"]), port));
    //        cancellationTokenSource = new CancellationTokenSource();
    //        Task.Factory.StartNew(() => ReceivingLoop(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
    //    }

    //    public override List<GenericDevice> SearchDevices()
    //    {
    //        Debug.WriteLine("Searching");
    //        searchDevices = new List<GenericDevice>();
    //        InternalTextReceived += Search_TextReceived;
    //        SendText("Details", null);
    //        Thread.Sleep(5000);
    //        InternalTextReceived -= Search_TextReceived;
    //        return searchDevices;
    //    }

    //    private void Search_TextReceived(string msg, IPAddress sender)
    //    {
    //        Debug.WriteLine(msg);
    //        if(msg.StartsWith("DetailsResponse"))
    //        {
    //            try
    //            {
    //                UDPDevice device = new UDPDevice() { IPAddress = sender };
    //                GenericDeviceParser.Parse(msg.Substring("DetailsResponse ".Length), device);
    //                searchDevices.Add(device);
    //            }
    //            catch
    //            {

    //            }
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        cancellationTokenSource.Cancel();
    //    }
        
    //    protected override void SendText(string text, TextGenericDevice target)
    //    {
    //        socket.SendTo(Encoding.Default.GetBytes(text), new IPEndPoint((target as UDPDevice)?.IPAddress ?? IPAddress.Broadcast, port));
    //    }

    //    private void ReceivingLoop(CancellationToken cancellationToken)
    //    {
    //        while(!cancellationToken.IsCancellationRequested)
    //        {
    //            EndPoint endPoint = new IPEndPoint(0, 0);
    //            int count = socket.ReceiveFrom(receivingBuffer, ref endPoint);
    //            string text = Encoding.Default.GetString(receivingBuffer, 0, count);
    //            InternalTextReceived?.Invoke(text, ((IPEndPoint)endPoint).Address);
    //        }
    //    }
    //}
}
