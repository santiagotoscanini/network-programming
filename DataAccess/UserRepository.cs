using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Domain;

namespace DataAccess
{
    public class UserRepository
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

        public void Delete(User user)
        {
            _users.Remove(user);
        }

        public void AddUserImage(User user, Image image)
        {
            user.Images.Add(image);
        }

        public List<Image> GetUserImages(User user)
        {
            return user.Images;
        }

        public void AddImageComment(Image image, Comment comment)
        {
            image.Comments.Add(comment);
        }

        public List<Comment> GetImageComments(Image image)
        {
            return image.Comments;
        }

        public void UpdateUser(User user)
        {
            var original = _users.Find(u => u.Equals(user));
            original.Password = user.Password;
        }
    }
}
