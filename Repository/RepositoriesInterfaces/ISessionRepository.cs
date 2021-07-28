using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.RepositoriesInterfaces
{
    public interface ISessionRepository
    {
        void AddLoggedUser(string userEmail);

        void DeleteLoggedUser(LoggedUser user);

        List<LoggedUser> GetLoggedUsers();
    }
}
