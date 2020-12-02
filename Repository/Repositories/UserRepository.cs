using Domain;
using Repository.RepositoriesInterfaces;
using System.Collections.Generic;

namespace Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private List<User> _users { get; } = new List<User>();

        public List<User> GetUsers()
        {
            return _users;
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void Delete(string userEmail)
        {
            var userSaved = _users.Find(u => u.Email.Equals(userEmail));
            _users.Remove(userSaved);
        }

        public void AddUserImage(User user, Image image)
        {
            var userSaved = _users.Find(u => u.Email.Equals(user.Email));
            userSaved.Images.Add(image);
        }

        public List<Image> GetUserImages(string userEmail)
        {
            var userSaved = _users.Find(u => u.Email.Equals(userEmail));
            return userSaved.Images;
        }

        public void AddImageComment(string userEmail, string imageName, Comment comment)
        {
            var user = _users.Find(u => u.Email.Equals(userEmail));
            var image = user.Images.Find(i => i.Name.Equals(imageName));
            image.Comments.Add(comment);
        }

        public List<Comment> GetImageComments(string userEmail, string imageName)
        {
            var user = _users.Find(u => u.Email.Equals(userEmail));
            var image = user.Images.Find(i => i.Name.Equals(imageName));
            return image.Comments;
        }

        public void UpdateUserPassword(User user)
        {
            var original = _users.Find(u => u.Equals(user));
            original.Password = user.Password;
        }
    }
}
