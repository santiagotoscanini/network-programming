﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkCommunication;

namespace Client
{
    class Client
    {
        private static bool _isConnected = false;
        
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
            _isConnected = true;
            new Thread(() => Read(communication)).Start();
            Write(communication);
        }

        static void Write(Communication communication)
        {
            while (_isConnected)
            {
                var msg = Console.ReadLine();
                ExecuteUserRequest(msg,  communication);
            }
        }

        static void Read(Communication communication)
        {
            while (_isConnected)
            {
                byte[] dataLength = communication.Read(ProtocolConstants.FixedDataSize);
                int dataSize = BitConverter.ToInt32(dataLength, 0);
                byte[] data = communication.Read(dataSize);
                var msg = Encoding.UTF8.GetString(data);
                Console.WriteLine(msg);
            }
        }
        
        private static void ExecuteUserRequest(string message, Communication communication)
        {
            try
            {

                var words = message.Split(" ");
                var verb = words[0];
                string element;
                switch (verb)
                {
                    case "put":
                        element = words[1];
                        switch (element)
                        {
                            case "image":
                                var path = words[3];
                                WriteToServer(communication, message);
                                communication.SendFile(path);
                                Console.WriteLine("Successfully sent file");
                                break;
                            default:
                                WriteToServer(communication, message);
                                break;
                        }
                        break;
                    case "post":
                        element = words[1];
                        switch (element)
                        {
                            case "logout":
                                WriteToServer(communication, message);
                                _isConnected = false;
                                break;
                            default:
                                WriteToServer(communication, message);
                                break;
                        }
                        break;
                    default:
                        WriteToServer(communication, message);
                        break;
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                Console.WriteLine("Bad Request");
            }
        }

        private static void WriteToServer(Communication communication, string msg)
        {
            var data = Encoding.UTF8.GetBytes(msg);
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            communication.Write(dataLength);
            communication.Write(data);
        }
    }
}