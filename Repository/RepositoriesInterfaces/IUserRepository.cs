using Domain;
using System.Collections.Generic;

namespace Repository.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();

        void AddUser(User user);

        void Delete(string userEmail);

        void AddUserImage(User user, Image image);

        List<Image> GetUserImages(string userEmail);

        void AddImageComment(string userEmail, string imageName, Comment comment);

        List<Comment> GetImageComments(string userEmail, string imageName);

        void UpdateUserPassword(User user);
    }
}
