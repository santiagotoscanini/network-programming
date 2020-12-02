using System.Threading.Tasks;
using Grpc.Core;
using LogServer.LoggerRepositoryInterface;
using Microsoft.Extensions.Logging;

namespace LogServer
{
    public class LoggerService : LoggerManager.LoggerManagerBase
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly ILogRepository _logRepository;

        public LoggerService(ILogger<LoggerService> logger, ILogRepository logRepository)
        {
            _logger = logger;
            _logRepository = logRepository;
        }

        public override Task<LogsResponse> GetLogs(EmptyMessage request, ServerCallContext context)
        {
            var logResponse = new LogsResponse();
            logResponse.Logs.AddRange(_logRepository.GetLogs());
            return Task.FromResult(logResponse);
        }
    }
}
