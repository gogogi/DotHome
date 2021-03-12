using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RWConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //int PORT = 8000;
            //UdpClient udpClient = new UdpClient();
            //udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.101"), PORT));


            



            ////Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            ////socket.EnableBroadcast = true;
            ////socket.Bind(new IPEndPoint(IPAddress.Any, 8000));

            //while (true)
            //{
            //    Console.ReadKey();
            //    var data = Encoding.UTF8.GetBytes("Details");
            //    udpClient.Send(data, data.Length, "255.255.255.255", PORT);

                
            //    //socket.SendTo(Encoding.Default.GetBytes("Ping"), new IPEndPoint(IPAddress.Parse("255.255.255.255"), 8000));
            //    //socket.SendTo(Encoding.Default.GetBytes("Ping"), new IPEndPoint(IPAddress.Parse("192.168.1.110"), 8000));
            //}
            CreateHostBuilder(args).ConfigureAppConfiguration(b => b.Add(new RWConfigurationSource("rwsettings.json"))).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
