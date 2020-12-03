using AdminServer.ServiceInterface;
using Domain;
using Grpc.Net.Client;
using Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Service
{
    public class UserService : IUserService
    {
        private RepoUser.RepoUserClient _clientRepository = new RepoUser.RepoUserClient(GrpcChannel.ForAddress("https://localhost:5003"));
        private const string _userAlreadyExistMessage = "There is already a registered user with the email ";
        private const string _userNotExistMessage = "User does not exist ";

        public async Task AddUserAsync(User user)
        {
            var alreadyExist = await UserAlreadyExistAsync(user.Email);
            if (alreadyExist)
            {
                throw new InvalidOperationException(_userAlreadyExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
                Password = user.Password,
            };
            await _clientRepository.AddUserAsync(userRequest);
        }

        private async Task<bool> UserAlreadyExistAsync(string email)
        {
            var users = await _clientRepository.GetUsersAsync(new EmptyMessagee { });
            return users.Users.Any(u => u.UserEmail.Equals(email));
        }

        public async Task UpdateUserAsync(User user)
        {
            var alreadyExist = await UserAlreadyExistAsync(user.Email);
            if (!alreadyExist)
            {
                throw new InvalidOperationException(_userNotExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
                Password = user.Password,
            };
            await _clientRepository.UpdateUserPasswordAsync(userRequest);
        }

        public async Task DeleteUserAsync(User user)
        {
            var alreadyExist = await UserAlreadyExistAsync(user.Email);
            if (!alreadyExist)
            {
                throw new InvalidOperationException(_userNotExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
            };
            await _clientRepository.DeleteUserAsync(userRequest);
        }
    }
}
