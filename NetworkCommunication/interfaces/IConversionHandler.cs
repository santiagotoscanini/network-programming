namespace NetworkCommunication.interfaces
{
    public interface IConversionHandler
    {
        byte[] ConvertStringToBytes(string value);
        string ConvertBytesToString(byte[] value);
        byte[] ConvertIntToBytes(int value);
        int ConvertBytesToInt(byte[] value);
        byte[] ConvertLongToBytes(long value);
        long ConvertBytesToLong(byte[] value);
    }
}