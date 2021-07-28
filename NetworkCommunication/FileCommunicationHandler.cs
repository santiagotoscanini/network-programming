using System;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileCommunicationHandler : IFileCommunicationHandler
    {
        private readonly IConversionHandler _conversionHandler;
        private readonly IFileUtils _fileUtils;
        private readonly Communication _communication;
        private readonly IFileStreamHandler _fileStreamHandler;
        
        public FileCommunicationHandler(Communication communication)
        {
            _conversionHandler = new ConversionHandler();
            _fileUtils = new FileUtils();
            _communication = communication;
            _fileStreamHandler = new FileStreamHandler(_fileUtils);
        }
        
        public void SendFile(string path)
        {
            if (_fileUtils.FileExists(path))
            {
                var fileName = _fileUtils.GetFileName(path);
                _communication.Write(_conversionHandler.ConvertIntToBytes(fileName.Length));
                _communication.Write(_conversionHandler.ConvertStringToBytes(fileName));
                
                var fileSize = _fileUtils.GetFileSize(path);
                _communication.Write(_conversionHandler.ConvertLongToBytes(fileSize));
                SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("Image doesn't exist");
            }
        }
        
        private void SendFileWithStream(long fileSize, string path)
        {
            var fileParts = ProtocolConstants.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                var packetSize = currentPart == fileParts ? (int) (fileSize - offset) : ProtocolConstants.MaxPacketSize;

                var data = _fileStreamHandler.Read(path, offset, packetSize);
                offset += packetSize;

                _communication.Write(data);
                currentPart++;
            }
        }

        public string ReceiveFile()
        {
            var fileNameSize = _conversionHandler.ConvertBytesToInt(
                _communication.Read(ProtocolConstants.FixedNameSize)
                );
            var fileName = _conversionHandler.ConvertBytesToString(
                _communication.Read(fileNameSize)
                );
            
            var fileSize = _conversionHandler.ConvertBytesToLong(
                _communication.Read(ProtocolConstants.FixedFileSize)
                );
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
                var packetSize = currentPart == fileParts ? (int) (fileSize - offset) : ProtocolConstants.MaxPacketSize;
                
                var data = _communication.Read(packetSize);
                offset += packetSize;
                
                _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}
