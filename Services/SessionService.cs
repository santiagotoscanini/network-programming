using System.Collections.Generic;
using DataAccess;
using Domain;

namespace Services
{
    public class SessionService
    {
        private SessionRepository SessionRepository { get; }
        private UserService UserService { get; }

        public SessionService(SessionRepository sessionRepository, UserService userService)
        {
            SessionRepository = sessionRepository;
            UserService = userService;
        }

        public void LoginUser(string userEmail, string userPassword)
        {
            UserService.ValidateUser(userEmail, userPassword);
            SessionRepository.AddLoggedUser(userEmail);
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

        private string ConvertLoggedUserListToString(List<LoggedUser> loggedUserList)
        {
            var list = "";
            foreach (var value in loggedUserList)
            {
                list += "|" + value.Email + " - " + value.ConnectionDate;
            }

            return list;
        }
    }
}