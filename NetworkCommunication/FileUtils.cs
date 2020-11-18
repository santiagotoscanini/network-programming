using System;
using System.IO;
using NetworkCommunication.interfaces;

namespace NetworkCommunication
{
    public class FileUtils : IFileUtils
    {
        private const string ImageDontExistMessage = "Image doesn't exist";
        
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

            throw new Exception(ImageDontExistMessage);
        }

        public long GetFileSize(string path)
        {
            if (FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception(ImageDontExistMessage);
        }

        public byte[] ReadFile(string path)
        {
            if (FileExists(path))
            {
                return File.ReadAllBytes(path);
            }

            throw new Exception(ImageDontExistMessage);
        }

        public void WriteFile(string fileName, byte[] data)
        {
            if (FileExists(fileName))
            {
                throw new Exception(ImageDontExistMessage);
            }

            File.WriteAllBytes(fileName, data);
        }
    }
}
