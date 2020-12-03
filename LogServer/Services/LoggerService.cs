using System.Threading.Tasks;
using Grpc.Core;
using LogServer.LoggerRepository;
using Microsoft.Extensions.Logging;

namespace LogServer
{
    public class LoggerService : LoggerManager.LoggerManagerBase
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public override Task<LogsResponse> GetLogs(EmptyMessage request, ServerCallContext context)
        {
            var logResponse = new LogsResponse();
            logResponse.Logs.AddRange(LogRepository.Instance().GetLogs());
            return Task.FromResult(logResponse);
        }
    }
}