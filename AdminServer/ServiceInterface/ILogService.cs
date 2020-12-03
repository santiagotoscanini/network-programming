using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.ServiceInterface
{
    public interface ILogService
    {
        ICollection<string> GetLogs();
    }
}
