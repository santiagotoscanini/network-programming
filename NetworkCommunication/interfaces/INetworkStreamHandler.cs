namespace NetworkCommunication.interfaces
{
    public interface INetworkStreamHandler
    {
        void Write(byte[] data);
        byte[] Read(int length);
    }
}