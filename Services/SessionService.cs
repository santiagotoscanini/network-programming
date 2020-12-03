using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Grpc.Net.Client;
using Repository;

namespace Services
{
    public class SessionService
    {
        private UserService UserService { get; }
        private Repo.RepoClient _clientRepository = new Repo.RepoClient(GrpcChannel.ForAddress("https://localhost:5003"));

        public SessionService(UserService userService)
        {
            UserService = userService;
        }

        public bool LoginUser(string userEmail, string userPassword)
        {
            var isCorrectUser = UserService.ValidateUserAsync(userEmail, userPassword).Result;
            if (isCorrectUser)
            { 
                _clientRepository.AddLoggedUserAsync(new AddLoggedUserRequest { UserEmail = userEmail });
            }
            return isCorrectUser;
        }

        public void LogoutUserAsync(string email)
        {
            var response = _clientRepository.GetLoggedUsers(new EmptyMessage());
            var getLoggedUser = response.LoggedUsers.FirstOrDefault(u => u.Email.Equals(email));
            if (getLoggedUser != null)
            {
                _clientRepository.DeleteLoggedUser(getLoggedUser);
            }
        }

        public async Task<string> GetLoggedUsersAsync()
        {
            GetLoggedUsersResponse response = await _clientRepository.GetLoggedUsersAsync(new EmptyMessage());
            return ConvertGetLoggedUserListToString(response);
        }

        private string ConvertGetLoggedUserListToString(GetLoggedUsersResponse loggedUserList)
        {
            return loggedUserList.LoggedUsers.ToList().Aggregate("",
                (current, value) => current + ("|" + value.Email + " - " + value.ConnectionDate));
        }
    }
}