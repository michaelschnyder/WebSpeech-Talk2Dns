using System;
using System.Net;
using Talk2Dns.Core;

namespace Talk2Dns.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 53);

            var talk2DnsServer = new Talk2DnsServer(endpoint, "t2d.fas-net.ch");
            talk2DnsServer.Start();

            Console.WriteLine("Started...");

            Console.ReadLine();

            talk2DnsServer.Stop();
        }
    }
}
