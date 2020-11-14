using System.Collections.Generic;
using Domain;

namespace DataAccess
{
    public class SessionRepository
    {
        private readonly List<LoggedUser> _loggedUsers = new List<LoggedUser>();

        public void AddLoggedUser(string userEmail)
        {
            var loggedUser = new LoggedUser
            {
                Email = userEmail
            };
            _loggedUsers.Add(loggedUser);
        }

        public void DeleteLoggedUser(LoggedUser user)
        {
            _loggedUsers.Remove(user);
        }

        public List<LoggedUser> GetLoggedUsers()
        {
            return _loggedUsers;
        }
    }
}
