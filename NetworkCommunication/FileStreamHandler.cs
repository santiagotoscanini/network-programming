using System;
using System.IO;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileStreamHandler : IFileStreamHandler
    {
        private readonly IFileHandler _fileHandler;
        private const string _imageNotExistMassage = "Image doesn't exist";
        private const string _errorReadingFile = "Error reading file";

        public FileStreamHandler(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }
        
        public byte[] Read(string path, long offset, int length)
        {
            if (_fileHandler.FileExists(path))
            {
                var data = new byte[length];

                using var fs = new FileStream(path, FileMode.Open) { Position = offset };
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = fs.Read(data, bytesRead, length - bytesRead);
                    if (read == 0)
                        throw new Exception(_errorReadingFile);
                    bytesRead += read;
                }

                return data;
            }

            throw new Exception(_imageNotExistMassage);
        }

        public void Write(string fileName, byte[] data)
        {
            if (_fileHandler.FileExists(fileName))
            {
                using var fs = new FileStream(fileName, FileMode.Append);
                fs.Write(data, 0, data.Length);
            }
            else
            {
                using var fs = new FileStream(fileName, FileMode.Create);
                fs.Write(data, 0, data.Length);
            }
        }
    }
}