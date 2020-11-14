using System;
using System.Net.Sockets;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileCommunicationHandler : IFileCommunicationHandler
    {
        private readonly IConversionHandler _conversionHandler;
        private readonly IFileHandler _fileHandler;
        private readonly INetworkStreamHandler _networkStreamHandler;
        private readonly IFileStreamHandler _fileStreamHandler;
        
        public FileCommunicationHandler(NetworkStream networkStream)
        {
            _conversionHandler = new ConversionHandler();
            _fileHandler = new FileHandler();
            _networkStreamHandler = new NetworkStreamHandler(networkStream);
            _fileStreamHandler = new FileStreamHandler(_fileHandler);
        }
        
        public void SendFile(string path)
        {
            if (_fileHandler.FileExists(path))
            {
                var fileName = _fileHandler.GetFileName(path);
                _networkStreamHandler.Write(_conversionHandler.ConvertIntToBytes(fileName.Length));
                _networkStreamHandler.Write(_conversionHandler.ConvertStringToBytes(fileName));
                
                long fileSize = _fileHandler.GetFileSize(path);
                _networkStreamHandler.Write(_conversionHandler.ConvertLongToBytes(fileSize));
                SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("Image doesn't exist");
            }
        }
        
        private void SendFileWithStream(long fileSize, string path)
        {
            long fileParts = ProtocolConstants.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int) (fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _fileStreamHandler.Read(path, offset, ProtocolConstants.MaxPacketSize);
                    offset += ProtocolConstants.MaxPacketSize;
                }

                _networkStreamHandler.Write(data);
                currentPart++;
            }
        }

        public string ReceiveFile()
        {
            int fileNameSize = _conversionHandler.ConvertBytesToInt(
                _networkStreamHandler.Read(ProtocolConstants.FixedNameSize));
            string fileName = _conversionHandler.ConvertBytesToString(_networkStreamHandler.Read(fileNameSize));
            long fileSize = _conversionHandler.ConvertBytesToLong(
                _networkStreamHandler.Read(ProtocolConstants.FixedFileSize));
            ReceiveFileWithStreams(fileSize, fileName);
            return fileName;
        }
        
        private void ReceiveFileWithStreams(long fileSize, string fileName)
        {
            long fileParts = ProtocolConstants.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _networkStreamHandler.Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _networkStreamHandler.Read(ProtocolConstants.MaxPacketSize);
                    offset += ProtocolConstants.MaxPacketSize;
                }
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}