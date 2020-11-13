using System;
using System.IO;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileHandler : IFileHandler
    {
        private const string _imageNotExistMessage = "Image doesn't exist";
        
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string GetFileName(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception(_imageNotExistMessage);
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception(_imageNotExistMessage);
        }

        public byte[] ReadFile(string path)
        {
            if (FileExists(path))
            {
                return File.ReadAllBytes(path);
            }

            throw new Exception(_imageNotExistMessage);
        }

        public void WriteFile(string fileName, byte[] data)
        {
            if (FileExists(fileName))
            {
                throw new Exception(_imageNotExistMessage);
                
            }
            else
            {
                File.WriteAllBytes(fileName, data);
            }
        }
    }
}