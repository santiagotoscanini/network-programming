using System.Collections.Generic;

namespace DataAccess
{
    public class SessionRepository
    {
        private readonly List<string> _loggedUsers = new List<string>();

        public void AddLoggedUser(string userEmail)
        {
            _loggedUsers.Add(userEmail);
        }

        public void DeleteLoggedUser(string userEmail)
        {
            _loggedUsers.Remove(userEmail);
        }

        public List<string> GetLoggedUsers()
        {
            return _loggedUsers;
        }
    }
}
