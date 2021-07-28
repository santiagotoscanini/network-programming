using System;
using System.IO;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileStreamHandler : IFileStreamHandler
    {
        private readonly IFileUtils _fileUtils;
        private const string ImageNotExistMassage = "Image doesn't exist";
        private const string ErrorReadingFile = "Error reading file";

        public FileStreamHandler(IFileUtils fileUtils)
        {
            _fileUtils = fileUtils;
        }
        
        public byte[] Read(string path, long offset, int length)
        {
            if (!_fileUtils.FileExists(path)) throw new Exception(ImageNotExistMassage);
            var data = new byte[length];

            using var fs = new FileStream(path, FileMode.Open) { Position = offset };
            var bytesRead = 0;
            while (bytesRead < length)
            {
                var read = fs.Read(data, bytesRead, length - bytesRead);
                if (read == 0)
                    throw new Exception(ErrorReadingFile);
                bytesRead += read;
            }

            return data;

        }

        public void Write(string fileName, byte[] data)
        {
            if (_fileUtils.FileExists(fileName))
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
