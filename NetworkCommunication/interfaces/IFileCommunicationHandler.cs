﻿namespace NetworkCommunication.interfaces
{
    public interface IFileCommunicationHandler
    {
        void SendFile(string path);
        string ReceiveFile();
    }
}