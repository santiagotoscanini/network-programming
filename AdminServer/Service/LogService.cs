using AdminServer.ServiceInterface;
using Grpc.Net.Client;
using LogServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Service
{
    public class LogService : ILogService
    {
        private LoggerManager.LoggerManagerClient _clientRepository = new LoggerManager.LoggerManagerClient(GrpcChannel.ForAddress("https://localhost:5002"));

        public ICollection<string> GetLogs()
        {
            var response = _clientRepository.GetLogs(new EmptyMessage { });
            return response.Logs;
        }
    }
}
