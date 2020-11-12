using System;
using System.Net.Sockets;

namespace NetworkCommunication
{
    public class Communication
    {
        private readonly NetworkStream _networkStream;

        public Communication(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public void Write(byte[] data)
        {
            _networkStream.Write(data, 0, data.Length);
        }

        public byte[] Read(int length)
        {
            var offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                var read = _networkStream.Read(data, offset, length - offset);
                if (read == 0)
                {
                    throw new Exception("Connection lost");
                }
                offset += read;
            }

            return data;
        }

        public void Disconnect()
        {
            _networkStream.Close();
        }

        public void ReceiveFile()
        {
            
        }
    }
}
