using Domain;
using System.Threading.Tasks;

namespace AdminServer.ServiceInterface
{
    public interface IUserService
    {
        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);
    }
}
