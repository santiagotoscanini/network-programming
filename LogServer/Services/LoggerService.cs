using System;
using System.Linq;
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

        public override Task<LogsResponse> GetLogs(PaginationRequest request, ServerCallContext context)
        {
            var logResponse = new LogsResponse();
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                return Task.FromResult(logResponse);
            }

            var logs = LogRepository.Instance().GetLogs();


            var offset = (request.Page - 1) * request.PageSize;
            if (offset > logs.Count)
            {
                return Task.FromResult(logResponse);
            }

            int lessCount = logs.Count - (request.Page * request.PageSize);
            var count = lessCount >= 0 ? request.PageSize : Math.Abs(request.PageSize + lessCount);

            logResponse.Logs.AddRange(logs.ToList().GetRange(offset, count));
            return Task.FromResult(logResponse);
        }
    }
}