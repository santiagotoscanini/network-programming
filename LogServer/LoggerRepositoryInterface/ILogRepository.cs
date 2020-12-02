
using System.Collections.Generic;

namespace LogServer.LoggerRepositoryInterface
{
    public interface ILogRepository
    {
        void AddLog(string log);

        ICollection<string> GetLogs(); 
    }
}
