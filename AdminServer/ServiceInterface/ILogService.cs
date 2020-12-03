using AdminServer.Helpers;
using System.Threading.Tasks;

namespace AdminServer.ServiceInterface
{
    public interface ILogService
    {
        Task<PaginatedResponse<string>> GetLogsAsync(int page, int pageSize);
    }
}
