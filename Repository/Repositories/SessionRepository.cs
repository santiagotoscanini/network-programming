using Domain;
using Repository.RepositoriesInterfaces;
using System.Collections.Generic;

namespace Repository.Repositories
{
    public class SessionRepository : ISessionRepository
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
