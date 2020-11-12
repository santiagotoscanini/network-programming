using System.Collections.Generic;
using DataAccess;

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
            var isUserLoged = SessionRepository.GetLoggedUsers().Contains(email);
            if (isUserLoged)
            {
                SessionRepository.DeleteLoggedUser(email);
            }
        }

        public string GetLoggedUsers()
        {
            return ConvertStringListToString(SessionRepository.GetLoggedUsers());
        }

        private string ConvertStringListToString(List<string> stringList)
        {
            var list = "";
            foreach (var value in stringList)
            {
                list = value + " ";
            }

            return list;
        }
    }
}