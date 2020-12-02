
using Domain;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Repository.RepositoriesInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class RepositoryUserService : RepoUser.RepoUserBase
    {
        private readonly ILogger<RepositoryService> _logger;
        private readonly IUserRepository _userRepository;

        public RepositoryUserService(ILogger<RepositoryService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        public override Task<EmptyMessagee> AddUser(AddUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Email = request.UserEmail,
                Password = request.Password,
            };
            _userRepository.AddUser(user);
            return Task.FromResult(new EmptyMessagee { });
        }

        public override Task<GetUsersResponse> GetUsers(EmptyMessagee request, ServerCallContext context)
        {
            var regiterUsers = _userRepository.GetUsers();
            
            var getUsersResponse = new GetUsersResponse();

            foreach (User user in regiterUsers) {
                ICollection<ImageProto> images = new List<ImageProto>();
                IEnumerable<CommentProto> comments = new List<CommentProto>().ToArray();
                foreach (Image image in user.Images)
                {
                    comments = image.Comments.Select(c => new CommentProto
                    {
                        Text = c.Text,
                        UserEmail = c.UserEmail,
                    });
                    var imageToSave = new ImageProto
                    {
                        Name = image.Name
                    };
                    imageToSave.Comments.AddRange(comments);
                    images.Add(imageToSave);
                };
                var userToSave = new UserProto {
                    UserEmail = user.Email,
                    Password = user.Password,
                };
                userToSave.Images.AddRange(images);
                getUsersResponse.Users.Add(userToSave);
            }
            return Task.FromResult(getUsersResponse);
        }

        public override Task<EmptyMessagee> DeleteUser(AddUserRequest request, ServerCallContext context)
        {
            _userRepository.Delete(request.UserEmail);
            return Task.FromResult(new EmptyMessagee { });
        }

        public override Task<EmptyMessagee> UpdateUserPassword(AddUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Email = request.UserEmail,
                Password = request.Password,
            };
            _userRepository.UpdateUserPassword(user);
            return Task.FromResult(new EmptyMessagee { });
        }

        public override Task<EmptyMessagee> AddUserImage(AddUserImageRequest request, ServerCallContext context)
        {
            var user = new User { Email = request.Email };
            var image = new Image { Name = request.ImageName };
            _userRepository.AddUserImage(user, image);
            return Task.FromResult(new EmptyMessagee { });
        }

        public override Task<GetUserImagesResponse> GetUserImages(AddUserRequest request, ServerCallContext context)
        {
            var userImages = _userRepository.GetUserImages(request.UserEmail);
            var getUserImagesResponse = new GetUserImagesResponse();
            IEnumerable<CommentProto> comments = new List<CommentProto>().ToArray();
            foreach (Image image in userImages)
            {
                comments = image.Comments.Select(c => new CommentProto
                {
                    Text = c.Text,
                    UserEmail = c.UserEmail,
                });
                var imageToSave = new ImageProto
                {
                    Name = image.Name
                };
                imageToSave.Comments.AddRange(comments);
                getUserImagesResponse.Images.Add(imageToSave);
            };
            return Task.FromResult(getUserImagesResponse);
        }

        public override Task<EmptyMessagee> AddImageComment(AddImageCommentRequest request, ServerCallContext context)
        {
            var comment = new Comment
            {
                Text = request.Comment.Text,
                UserEmail = request.Comment.UserEmail,
            };
            _userRepository.AddImageComment(request.UserEmail, request.ImageName, comment);
            return Task.FromResult(new EmptyMessagee { });
        }

        public override Task<GetImageCommentsResponse> GetImageComments(AddUserImageRequest request, ServerCallContext context)
        {
            var comments = _userRepository.GetImageComments(request.Email, request.ImageName);
            var getImageCommentsResponse = new GetImageCommentsResponse();
            getImageCommentsResponse.Comments.AddRange(comments.Select(c => new CommentProto
            {
                Text = c.Text,
                UserEmail = c.UserEmail,
            }));
            return Task.FromResult(getImageCommentsResponse);
        }
    }
}
