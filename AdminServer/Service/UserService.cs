using AdminServer.ServiceInterface;
using Domain;
using Grpc.Net.Client;
using Repository;
using System;
using System.Linq;

namespace AdminServer.Service
{
    public class UserService : IUserService
    {
        private RepoUser.RepoUserClient _clientRepository = new RepoUser.RepoUserClient(GrpcChannel.ForAddress("https://localhost:5003"));
        private const string _userAlreadyExistMessage = "There is already a registered user with the email ";
        private const string _userNotExistMessage = "User does not exist ";

        public void AddUser(User user)
        {
            var alreadyExist = UserAlreadyExist(user.Email);
            if (alreadyExist)
            {
                throw new Exception(_userAlreadyExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
                Password = user.Password,
            };
            _clientRepository.AddUserAsync(userRequest);
        }

        private bool UserAlreadyExist(string email)
        {
            var users = _clientRepository.GetUsers(new EmptyMessagee { });
            return users.Users.Select(u => u.UserEmail.Equals(email)).Any();
        }

        public void UpdateUser(User user)
        {
            var alreadyExist = UserAlreadyExist(user.Email);
            if (!alreadyExist)
            {
                throw new InvalidOperationException(_userNotExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
                Password = user.Password,
            };
            _clientRepository.UpdateUserPasswordAsync(userRequest);
        }

        public void DeleteUser(User user)
        {
            var alreadyExist = UserAlreadyExist(user.Email);
            if (!alreadyExist)
            {
                throw new InvalidOperationException(_userNotExistMessage + user.Email);
            }
            var userRequest = new AddUserRequest
            {
                UserEmail = user.Email,
            };
            _clientRepository.DeleteUserAsync(userRequest);
        }
    }
}
