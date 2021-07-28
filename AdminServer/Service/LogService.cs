using AdminServer.Helpers;
using AdminServer.ServiceInterface;
using Grpc.Net.Client;
using LogServer;
using System;
using static Constants.Constants;
using System.Threading.Tasks;

namespace AdminServer.Service
{
    public class LogService : ILogService
    {
        private LoggerManager.LoggerManagerClient _clientRepository = new LoggerManager.LoggerManagerClient(GrpcChannel.ForAddress(String.Format(UriToFormat, localhostHttps, LogServerPort)));

        public async Task<PaginatedResponse<string>> GetLogsAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return new PaginatedResponse<string>();
            }
            var response = await _clientRepository.GetLogsAsync(new PaginationRequest 
            {
                Page = page,
                PageSize = pageSize,
            });
            
            return new PaginatedResponse<string>
            {
                TotalElements = response.Logs.Count,
                TotalPages = (int)Math.Ceiling((double)response.Logs.Count / pageSize),
                Elements = response.Logs,
            };
        }
    }
}
