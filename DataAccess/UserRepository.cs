using System.Collections.Generic;
using System.Linq;
using Domain;

namespace DataAccess
{
    public class UserRepository
    {
        private List<User> _users { get; } = new List<User>();
        
        public List<User> GetUsers()
        {
            return _users;
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void Delete(User user)
        {
            _users.Remove(user);
        }
    }
}
