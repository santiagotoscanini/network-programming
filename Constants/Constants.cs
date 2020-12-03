using System;

namespace Constants
{
    public static class Constants
    {
        public static readonly string localhost = "127.0.0.1";
        public static readonly string localhostHttps = "https://localhost";
        
        public static readonly int LogServerPort = 5002;

        public static readonly int RepositoryPort = 5003;

        public static readonly int AdminServerPort = 5004;

        public static readonly int TcpServerPort = 6000;
        public static readonly int MaxNumberOfTcpClients = 100;

        public static readonly string UriToFormat = "{0}:{1}";

        public static readonly string InfoLogFormat = "{0} - [INFO] - {1} - {2}";
    }
}
