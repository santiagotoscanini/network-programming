using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using Domain;

namespace Services
{
    public class UserService
    {
        private UserRepository UserRepository { get; }

        private const string InvalidUserDataMessage = "Invalid email or password";
        private const string UserAlreadyExistMessage = "Already exist user with that email";
        private const string WrongEmailMessage = "Wrong Email";

        public UserService(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        internal void ValidateUser(string userEmail, string userPassword)
        {
            try
            {
                var first = UserRepository.GetUsers()
                    .First(u => u.Email.Equals(userEmail) && u.Password.Equals(userPassword));
            }
            catch (Exception e)
            {
                throw new Exception(InvalidUserDataMessage);
            }
        }

        public string GetUsers()
        {
            return ConvertUserListToString(UserRepository.GetUsers());
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
            UserRepository.AddUser(user);
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
            var user = GetUserByEmail(email);
            UserRepository.Delete(user);
        }

        public void AddImageComment(string commentText, string imageName, string userEmail, string userCommentEmail)
        {
            var user = GetUserByEmail(userEmail);
            var userComment = GetUserByEmail(userCommentEmail);
            var image = user.Images.Find(i => i.Name.Equals(imageName));
            var comment = new Comment
            {
                Text = commentText,
                User = userComment,
            };
            UserRepository.AddImageComment(image, comment);
        }

        public string GetUserImages(string userEmail)
        {
            var user = GetUserByEmail(userEmail);

            var images = UserRepository.GetUserImages(user);
            return ConvertImageListToString(images);
        }
        
        private string ConvertImageListToString(IEnumerable<Image> images)
        {
            var userImages = images.Select(i => i.Name);

            return userImages.Aggregate("", (current, image) => current + image + " ");
        }
        
        public string GetImageComments(string userEmail, string imageName)
        {
            var user = GetUserByEmail(userEmail);
            var image = GetUserImageByName(imageName, user);
            var comments = UserRepository.GetImageComments(image);
            return ConvertCommentListToString(comments);
        }
        
        private string ConvertCommentListToString(IEnumerable<Comment> comments)
        {
            var imageComments = comments.Select(c => c.Text + "    by    " + c.User.Email);

            return imageComments.Aggregate("", (current, comment) => current + comment + " ");
        }
        
        public void AddUserImage(string imageName, string userEmail)
        {
            var image = new Image {Name = imageName};
            var user = GetUserByEmail(userEmail);
            UserRepository.AddUserImage(user, image);
        }

        public void UpdateUserPassword(string email, string password)
        {
            var user = GetUserByEmail(email);
            user.Password = password;
            UserRepository.UpdateUser(user);
        }

        private User GetUserByEmail(string userEmail)
        {
            var user = UserRepository.GetUsers().Find(u => u.Email.Equals(userEmail));
            if (user == null)
            {
                throw new Exception(WrongEmailMessage);
            }

            return user;
        }

        private Image GetUserImageByName(string imageName, User user)
        {
            var image = UserRepository.GetUserImages(user).Find(i => i.Name.Equals(imageName));
            if (image == null)
            {
                throw new Exception("Doesn't exist: "+ image.Name + " image");
            }

            return image;
        }
    }
}
