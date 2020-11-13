namespace NetworkCommunication.interfaces
{
    public interface IFileHandler
    {
        bool FileExists(string path);
        string GetFileName(string path);
        long GetFileSize(string path);
        byte[] ReadFile(string path);
        void WriteFile(string fileName, byte[] data);
    }
}