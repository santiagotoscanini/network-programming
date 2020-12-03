using System.Collections.Generic;

namespace LogServer.LoggerRepository
{
    public class LogRepository
    {
        private static LogRepository _instance;

        private ICollection<string> _logs = new List<string>();

        protected LogRepository() { }

        public static LogRepository Instance()
        {
            if(_instance == null)
            {
                _instance = new LogRepository();
            }
            return _instance;
        }

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
