using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Repository;
using static Constants.Constants;

namespace Services
{
    public class SessionService
    {
        private UserService UserService { get; }
        private Repo.RepoClient _clientRepository = new Repo.RepoClient(GrpcChannel.ForAddress(String.Format(UriToFormat, localhostHttps, RepositoryPort)));

        public SessionService(UserService userService)
        {
            UserService = userService;
        }

        public async Task<bool> LoginUserAsync(string userEmail, string userPassword)
        {
            var isCorrectUser = await UserService.ValidateUserAsync(userEmail, userPassword);
            if (isCorrectUser) await _clientRepository.AddLoggedUserAsync(new AddLoggedUserRequest { UserEmail = userEmail });
            return isCorrectUser;
        }

        public async Task LogoutUserAsync(string email)
        {
            var response = await _clientRepository.GetLoggedUsersAsync(new EmptyMessage());
            var getLoggedUser = response.LoggedUsers.FirstOrDefault(u => u.Email.Equals(email));
            if (getLoggedUser != null) await _clientRepository.DeleteLoggedUserAsync(getLoggedUser);
        }

        public async Task<string> GetLoggedUsersAsync()
        {
            return ConvertGetLoggedUserListToString(await _clientRepository.GetLoggedUsersAsync(new EmptyMessage()));
        }

        private string ConvertGetLoggedUserListToString(GetLoggedUsersResponse loggedUserList)
        {
            return loggedUserList.LoggedUsers.ToList().Aggregate("", (current, value) => current + ("|" + value.Email + " - " + value.ConnectionDate));
        }
    }
}
