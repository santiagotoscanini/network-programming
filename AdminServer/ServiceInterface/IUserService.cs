using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.ServiceInterface
{
    public interface IUserService
    {
        void AddUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);
    }
}
