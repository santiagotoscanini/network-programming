using System;

namespace Constants
{
    public static class Constants
    {
        private const string _logServerRoute = "https://localhost:5002";
        public static readonly string LogServerRoute = _logServerRoute;

        private const string _repositoryRoute = "https://localhost:5003";
        public static readonly string RepositoryRoute = _repositoryRoute;

        private const int _serverPort = 6000;
        public static readonly int ServerPort = _serverPort;
    }
}
