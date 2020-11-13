namespace NetworkCommunication
{
    public static class ProtocolConstants
    {
        public const int FixedDataSize = 4;
        public const int FixedNameSize = 4;
        public const int FixedFileSize = 8;
        public const int MaxPacketSize = 32768;

        public static long CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / MaxPacketSize;
            return fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1;
        }
    }
}