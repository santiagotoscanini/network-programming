using System.Collections.Generic;
using System.Linq;
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

        public void LogoutUser(string email)
        {
            var loggedUser = SessionRepository.GetLoggedUsers().Find(l => l.Email.Equals(email));
            if (loggedUser != null)
            {
                SessionRepository.DeleteLoggedUser(loggedUser);
            }
        }

        public string GetLoggedUsers()
        {
            return ConvertLoggedUserListToString(SessionRepository.GetLoggedUsers());
        }

        private string ConvertLoggedUserListToString(IEnumerable<LoggedUser> loggedUserList)
        {
            return loggedUserList.Aggregate("",
                (current, value) => current + ("|" + value.Email + " - " + value.ConnectionDate));
        }
    }
}