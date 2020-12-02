using LogServer.LoggerRepositoryInterface;
using System.Collections.Generic;

namespace LogServer.LoggerRepository
{
    public class LogRepository : ILogRepository
    {
        private ICollection<string> _logs = new List<string>();

        public void AddLog(string log)
        {
            _logs.Add(log);
        }

        public ICollection<string> GetLogs()
        {
            return _logs;
        }
    }
}
