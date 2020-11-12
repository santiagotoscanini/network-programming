using System;
using NetworkCommunication;

namespace CommunicationHelper
{
    public class Protocol
    {
        private Communication communication;
        
        public byte[] Read(int length)
        {
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                var recived = communication.Read()
            }
        }
    }
}