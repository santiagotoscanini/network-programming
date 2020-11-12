using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Domain;

namespace Services
{
    public class UserService
    {
        private UserRepository _userRepository { get; }

        private const string InvalidUserDataMessage = "Invalid email or password";
        private const string UserAlreadyExistMessage = "Already exist user with that email";

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        internal void ValidateUser(string userEmail, string userPassword)
        {
            try
            {
                _userRepository.GetUsers()
                    .First(u => u.Email.Equals(userEmail) && u.Password.Equals(userPassword));
            }
            catch (Exception e)
            {
                throw new Exception(InvalidUserDataMessage);
            }
        }

        public string GetUsers()
        {
            return ConvertUserListToString(_userRepository.GetUsers());
        }

        private string ConvertUserListToString(IEnumerable<User> users)
        {
            var usersEmails = users.Select(u => u.Email);

            return usersEmails.Aggregate("", (current, user) => current + user + " ");
        }

        public void CreateUser(string email, string password)
        {
            var user = new User
            {
                Email = email,
                Password = password,
            };
            VerifyUser(user);
            _userRepository.AddUser(user);
        }

        private void VerifyUser(User user)
        {
            var alreadyExist = GetUsers().Contains(user.Email);
            if (alreadyExist)
            {
                throw new Exception(UserAlreadyExistMessage);
            }
        }

        public void DeleteUser(string email)
        {
            var user = _userRepository.GetUsers().Find(u => u.Email.Equals(email));
            if (user == null)
            {
                throw new Exception("The user: "+ email+" doesn't exist");
            }
            _userRepository.Delete(user);
        }
    }
}