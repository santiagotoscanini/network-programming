using Domain;

namespace AdministratorServer.ServicesInterface
{
    public interface IUserService
    {
        void AddUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);
    }
}
