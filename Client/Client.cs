using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkCommunication;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting client...");
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            TcpClient client = new TcpClient(clientIpEndPoint);
            Console.WriteLine("Attempting connection to server...");
            client.Connect(serverIpEndPoint);
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
