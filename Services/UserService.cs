﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Grpc.Net.Client;
using Repository;
using static Constants.Constants;

namespace Services
{
    public class UserService
    {
        private RepoUser.RepoUserClient _clientRepository = new RepoUser.RepoUserClient(GrpcChannel.ForAddress(String.Format(UriToFormat, localhostHttps, RepositoryPort)));

        private const string UserAlreadyExistMessage = "Already exist user with that email";
        private const string WrongEmailMessage = "Wrong Email";

        public UserService() {}

        internal async System.Threading.Tasks.Task<bool> ValidateUserAsync(string userEmail, string userPassword)
        {
                var response = await _clientRepository.GetUsersAsync(new EmptyMessagee {});
                return response.Users.Any(u => u.UserEmail.Equals(userEmail) && u.Password.Equals(userPassword));
        }

        public async System.Threading.Tasks.Task<string> GetUsersAsync()
        {
            var response = await _clientRepository.GetUsersAsync(new EmptyMessagee());
            var convertFromResp = ConvertResponseToUser(response.Users);
            return ConvertUserListToString(convertFromResp);
        }

        private ICollection<User> ConvertResponseToUser(ICollection<UserProto> usersProtos) 
        {
            var users = usersProtos.Select(u => new User 
            {
                Email = u.UserEmail,
                Password = u.Password,
            });
            return users.ToArray();
        }

        private string ConvertUserListToString(IEnumerable<User> users)
        {
            var usersEmails = users.Select(u => u.Email);

            return usersEmails.Aggregate("", (current, user) => current + user + " ");
        }

        public async System.Threading.Tasks.Task<EmptyMessagee> CreateUserAsync(string email, string password)
        {
            var userRequest = new AddUserRequest
            {
                UserEmail = email,
                Password = password,
            };
            await VerifyUserAsync(userRequest);
            return await _clientRepository.AddUserAsync(userRequest);
        }

        private async System.Threading.Tasks.Task VerifyUserAsync(AddUserRequest user)
        {
            var users = await GetUsersAsync();
            bool alreadyExist = users.Contains(user.UserEmail);
            if (alreadyExist)
            {
                throw new Exception(UserAlreadyExistMessage);
            }
        }

        public async System.Threading.Tasks.Task<bool> DeleteUserAsync(string email)
        {
            var isValidUser = await ValidateUserByEmailAsync(email);
            if (isValidUser)
            {
                var userRequest = new AddUserRequest
                {
                    UserEmail = email,
                };
                await _clientRepository.DeleteUserAsync(userRequest);
            }
            return isValidUser;
        }

        public async System.Threading.Tasks.Task<bool> AddImageCommentAsync(string commentText, string imageName, string userEmail, string userCommentEmail)
        {
            var isValidUser = await ValidateUserByEmailAsync(userEmail);
            var isValidUserComment = await ValidateUserByEmailAsync(userCommentEmail);
            if (isValidUser && isValidUserComment) 
            {
                var imagesResponse = await _clientRepository.GetUserImagesAsync(new AddUserRequest { UserEmail = userEmail});
                var isValidImage = ValidateImageByName(imageName, imagesResponse.Images);
                if (isValidImage) 
                {
                    var comment = new CommentProto
                    {
                        Text = commentText,
                        UserEmail = userCommentEmail,
                    };
                    var addImageCommentRequest = new AddImageCommentRequest
                    {
                        UserEmail = userEmail,
                        ImageName = imageName,
                        Comment = comment
                    };
                    await _clientRepository.AddImageCommentAsync(addImageCommentRequest);
                }
                return isValidImage;
            }
            return isValidUser && isValidUserComment;
        }

        public async System.Threading.Tasks.Task<string> GetUserImagesAsync(string userEmail)
        {
            var isValidUser = await ValidateUserByEmailAsync(userEmail);
            if (isValidUser) 
            {
                var request = new AddUserRequest { UserEmail = userEmail };
                var imagesResponse = await _clientRepository.GetUserImagesAsync(request);
                return ConvertImageListToString(imagesResponse.Images);
            }
            return WrongEmailMessage;
        }
        
        private string ConvertImageListToString(IEnumerable<ImageProto> images)
        {
            var userImages = images.Select(i => i.Name);

            return userImages.Aggregate("", (current, image) => current + image + " ");
        }
        
        public async System.Threading.Tasks.Task<string> GetImageCommentsAsync(string userEmail, string imageName)
        {
            var isValidUser = await ValidateUserByEmailAsync(userEmail);
            if (isValidUser) {
                var imagesResponse = await _clientRepository.GetUserImagesAsync(new AddUserRequest { UserEmail = userEmail });
                if (ValidateImageByName(imageName, imagesResponse.Images))
                {
                    var request = new AddUserImageRequest 
                    { 
                        Email = userEmail,
                        ImageName = imageName,
                    };
                    var getImageCommentsResult = await _clientRepository.GetImageCommentsAsync(request);
                    return ConvertCommentListToString(getImageCommentsResult.Comments);
                }
                return "Invalid image name";
            }
            return WrongEmailMessage;
        }
        
        private string ConvertCommentListToString(IEnumerable<CommentProto> comments)
        {
            var imageComments = comments.Select(c => c.Text + "    by    " + c.UserEmail);

            return imageComments.Aggregate("", (current, comment) => current + comment + " ");
        }
        
        public async System.Threading.Tasks.Task<bool> AddUserImageAsync(string imageName, string userEmail)
        {
            var isValidUser = await ValidateUserByEmailAsync(userEmail);
            if (isValidUser)
            { 
                var addUserImageRequest = new AddUserImageRequest 
                {
                    Email = userEmail,
                    ImageName = imageName
                };
                await _clientRepository.AddUserImageAsync(addUserImageRequest);
            }
            return isValidUser;
        }

        public async System.Threading.Tasks.Task<bool> UpdateUserPasswordAsync(string email, string password)
        {
            var isValidUser = await ValidateUserByEmailAsync(email);
            if (isValidUser)
            {
                var userRequest = new AddUserRequest
                {
                    UserEmail = email,
                    Password = password,
                };
                await _clientRepository.UpdateUserPasswordAsync(userRequest);
            }
            return isValidUser;
        }

        private async System.Threading.Tasks.Task<bool> ValidateUserByEmailAsync(string userEmail)
        {
            var response = await _clientRepository.GetUsersAsync(new EmptyMessagee());
            var isValidUser = ConvertResponseToUser(response.Users).Any(u => u.Email.Equals(userEmail));
            return isValidUser;
        }

        private bool ValidateImageByName(string imageName, ICollection<ImageProto> images)
        {  
            var existImage = images.ToList().Any(i => i.Name.Equals(imageName));
            return existImage;
        }
    }
}
