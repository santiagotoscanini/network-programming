using AdminServer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.ServiceInterface
{
    public interface ILogService
    {
        PaginatedResponse<string> GetLogs(int page, int pageSize);
    }
}
