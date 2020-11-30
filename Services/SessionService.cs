using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Grpc.Net.Client;
using Repository;

namespace Services
{
    public class SessionService
    {
        private SessionRepository SessionRepository { get; }
        private UserService UserService { get; }
        private Repo.RepoClient _clientRepository = new Repo.RepoClient(GrpcChannel.ForAddress("https://localhost:5001"));

        public SessionService(SessionRepository sessionRepository, UserService userService)
        {
            SessionRepository = sessionRepository;
            UserService = userService;
        }

        public void LoginUser(string userEmail, string userPassword)
        {
            UserService.ValidateUser(userEmail, userPassword);
            SessionRepository.AddLoggedUser(userEmail);
            _clientRepository.AddLoggedUserAsync(new AddLoggedUserRequest { UserEmail = userEmail });
        }

        public async Task LogoutUserAsync(string email)
        {
            var loggedUser = SessionRepository.GetLoggedUsers().Find(l => l.Email.Equals(email));
            GetLoggedUsersResponse response = await _clientRepository.GetLoggedUsersAsync(new EmptyMessage());
            var getLoggedUser = response.LoggedUsers.FirstOrDefault(u => u.Email.Equals(email));
            if (loggedUser != null)
            {
                SessionRepository.DeleteLoggedUser(loggedUser);
            }
            if (getLoggedUser != null)
            {
                _clientRepository.DeleteLoggedUserAsync(getLoggedUser);
            }
        }

        public async Task<string> GetLoggedUsersAsync()
        {
            GetLoggedUsersResponse response = await _clientRepository.GetLoggedUsersAsync(new EmptyMessage());
            return ConvertLoggedUserListToString(SessionRepository.GetLoggedUsers()) + ConvertGetLoggedUserListToString(response);
        }

        private string ConvertLoggedUserListToString(IEnumerable<LoggedUser> loggedUserList)
        {
            return loggedUserList.Aggregate("",
                (current, value) => current + ("|" + value.Email + " - " + value.ConnectionDate));
        }

        private string ConvertGetLoggedUserListToString(GetLoggedUsersResponse loggedUserList)
        {
            return loggedUserList.LoggedUsers.ToList().Aggregate("",
                (current, value) => current + ("|" + value.Email + " - " + value.ConnectionDate));
        }
    }
}