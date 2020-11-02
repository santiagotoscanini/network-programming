using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkCommunication;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            TcpListener server = new TcpListener(ipEndPoint);
            server.Start(100);
            TcpClient client = server.AcceptTcpClient();
            server.Stop();
            NetworkStream stream = client.GetStream();
            var communication = new Communication(stream);
            new Thread(() => Read(communication)).Start();
            Write(communication);
        }

        static void Write(Communication communication)
        {
            while (true)
            {
                var msg = Console.ReadLine();
                var data = Encoding.UTF8.GetBytes(msg);
                byte[] dataLength = BitConverter.GetBytes(data.Length);
                communication.Write(dataLength);
                communication.Write(data);
            }
        }

        static void Read(Communication communication)
        {
            while (true)
            {
                byte[] dataLength = communication.Read(ProtocolConstants.FixedDataSize);
                int dataSize = BitConverter.ToInt32(dataLength, 0);
                byte[] data = communication.Read(dataSize);
                var msg = Encoding.UTF8.GetString(data);
                Console.WriteLine(msg);
            }
        }
    }
}
