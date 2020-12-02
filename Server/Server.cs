﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkCommunication;
using Services;

namespace Server
{
    class Server
    {
        private static readonly Dictionary<string, Communication> Clients = new Dictionary<string, Communication>();
        private static readonly UserService UserService = new UserService();
        private static bool _isConnected = true;
        private static readonly SessionService SessionService = new SessionService(UserService);

        private const string InvalidUserDataMessage = "Invalid email or password";
        private const string InvalidImageMessage = "Invalid image name";
        private const string InvalidUserEmailMessage = "Invalid user email";

        private const string CommandsToUser = @"'get users' -> to get the register users
'post logout <<your email>>' -> to close your session and close the connection
'put image <<your email>> <<path>' -> to save a new image
'get images <<email>>' -> to get user images
'put comment <<email>> <<image name>> <<your email>> <<comment>>' -> to add image comment
'get comments <<your email>> <<image name>>' -> to get the image comments";

        private const string CommandsToServer = @"'get connectedUsers' -> to get the connected users
'put user <<email>> <<password>>' -> to create a new user
'delete user <<email>>' -> to delete a user
'post user <<email>> <<new password>>' -> to change the user password
'bye' -> to shut down the server";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            new Thread(StartListen).Start();

            var server = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000));
            server.Start(100);
            
            WaitingForClients(server);
        }

        private static void StartListen()
        {
            Console.WriteLine("Server connected. Use the 'help' command to know what you can do.");
            _isConnected = true;
            while (_isConnected)
            {
                var request = Console.ReadLine();
                Console.WriteLine(ExecuteServerRequestAsync(request).Result);
            }
        }

        private static async System.Threading.Tasks.Task<string> ExecuteServerRequestAsync(string message)
        {
            try
            {
                var words = message.Split(" ");
                var verb = words[0];
                string element;

                switch (verb)
                {
                    case "help":
                        return CommandsToServer;
                    case "bye":
                        _isConnected = false;
                        return "Bye bye";
                    case "get":
                        element = words[1];
                        switch (element)
                        {
                            case "connectedUsers":
                                return await SessionService.GetLoggedUsersAsync();
                        }

                        break;
                    case "put":
                        element = words[1];
                        switch (element)
                        {
                            case "user":
                                var email = words[2];
                                var password = words[3];
                                return CreateUser(email, password);
                        }

                        break;
                    case "delete":
                        element = words[1];
                        switch (element)
                        {
                            case "user":
                                var email = words[2];
                                return DeleteUser(email);
                        }

                        break;
                    case "post":
                        element = words[1];
                        switch (element)
                        {
                            case "user":
                                var email = words[2];
                                var password = words[3];
                                return UpdateUserPassword(email, password);
                        }

                        break;
                }

                return "Bad request";
            }
            catch (IndexOutOfRangeException)
            {
                return "Bad Request";
            }
        }

        private static string ExecuteUserRequest(string message)
        {
            try
            {
                var words = message.Split(" ");
                var verb = words[0];
                string element;

                switch (verb)
                {
                    case "help":
                        return CommandsToUser;
                    case "get":
                        element = words[1];
                        switch (element)
                        {
                            case "users":
                                return UserService.GetUsersAsync().Result;
                            case "images":
                                var email = words[2];
                                return GetUserImages(email);
                            case "comments":
                                var userEmail = words[2];
                                var imageName = words[3];
                                return GetImageComments(userEmail, imageName);
                        }

                        break;
                    case "post":
                        element = words[1];
                        switch (element)
                        {
                            case "logout":
                                var email = words[2];
                                return Logout(email);
                        }

                        break;
                    case "put":
                        element = words[1];
                        switch (element)
                        {
                            case "image":
                                var email = words[2];
                                return ReceiveFile(email);
                            case "comment":
                                var userImageEmail = words[2];
                                var imageName = words[3];
                                var userEmail = words[4];
                                var comment = words[5];
                                return CreateImageComment(comment, imageName, userImageEmail,
                                    userEmail);
                        }

                        break;
                }

                return "Bad request";
            }
            catch (IndexOutOfRangeException)
            {
                return "Bad Request";
            }
        }

        private static void WaitingForClients(TcpListener server)
        {
            while (_isConnected)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                var communication = new Communication(stream);
                new Thread(() => StartingUserInteraction(communication)).Start();
            }
        }

        private static void StartingUserInteraction(Communication communication)
        {
            Write(communication,
                "Welcome, if you want to login in your account write: 'login' or if you want to register write: 'register'");
            string readInfo = ReadInfo(communication);
            switch (readInfo) 
            // TODO: arreglar esto
            {
                case "login":
                    Login(communication);
                    break;
                case "register":
                    Register(communication);
                    break;
                default:
                    StartingUserInteraction(communication);
                    break;
            }

            Write(communication,
                "You are connected to InstaFoto, write 'help' to know what commands you can execute. Enjoy!");
            Read(communication);
        }

        private static string Login(Communication communication)
        {
            Write(communication, "Write your email:");
            string userEmail = ReadInfo(communication);

            Write(communication, "Write your password:");
            string userPassword = ReadInfo(communication);
            
            var isCorrectUser = SessionService.LoginUser(userEmail, userPassword);
            if (isCorrectUser) 
            { 
                Clients.Add(userEmail, communication);
            }
            else { 
                Write(communication, InvalidUserDataMessage);
                // TODO: Cambiar esto
                return Login(communication);
            }

            return userEmail;
        }

        private static string Register(Communication communication)
        {
            Write(communication, "Write your email:");
            string userEmail = ReadInfo(communication);
            Write(communication, "Write your password:");
            string userPassword = ReadInfo(communication);
            try
            {
                UserService.CreateUser(userEmail, userPassword);
                SessionService.LoginUser(userEmail, userPassword);
                Clients.Add(userEmail, communication);
            }
            catch (Exception e)
            {
                Write(communication, e.Message);
                // Cambiar esto
                return Register(communication);
            }

            return userEmail;
        }

        private static string ReadInfo(Communication communication)
        {
            byte[] dataLength = communication.Read(ProtocolConstants.FixedDataSize);
            int dataSize = BitConverter.ToInt32(dataLength, 0);
            byte[] data = communication.Read(dataSize);
            var msg = Encoding.UTF8.GetString(data);
            return msg;
        }

        private static void Write(Communication communication, string message)
        {
            message = "> " + message;
            var data = Encoding.UTF8.GetBytes(message);
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            communication.Write(dataLength);
            communication.Write(data);
        }

        private static void Read(Communication communication)
        {
            while (true)
            {
                try
                {
                    byte[] dataLength = communication.Read(ProtocolConstants.FixedDataSize);
                    int dataSize = BitConverter.ToInt32(dataLength, 0);
                    byte[] data = communication.Read(dataSize);
                    var msg = Encoding.UTF8.GetString(data);
                    Write(communication, ExecuteUserRequest(msg));
                }
                catch (System.IO.IOException)
                {
                    return;
                }
            }
        }

        private static string CreateUser(string email, string password)
        {
            try
            {
                UserService.CreateUser(email, password);
                return "Created";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string Logout(string email)
        {
            try
            {
                SessionService.LogoutUserAsync(email);
                Clients.Remove(email);
                // TODO: cerrar el hilo de aca
                return "You session was closed and you are disconnected";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string DeleteUser(string email)
        {
            
            var isDeleted = UserService.DeleteUser(email);
            if (!isDeleted)
            {
                return InvalidUserEmailMessage;
            }
            return "User deleted";
           
        }

        private static string ReceiveFile(string email)
        {
            try
            {
                var imageName = Clients.GetValueOrDefault(email)?.ReceiveFile();
                var isSaved = UserService.AddUserImage(imageName, email);
                if (!isSaved)
                {
                    return InvalidUserEmailMessage;
                }
                return "Successfully saved";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string CreateImageComment(string commentText, string imageName, string userEmail,
            string userCommentEmail)
        {
            try
            {
                var isCommentSaved = UserService.AddImageCommentAsync(commentText, imageName, userEmail, userCommentEmail).Result;
                if (isCommentSaved)
                {
                    return "Comment Saved";
                }
                else
                {
                    return InvalidImageMessage;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string UpdateUserPassword(string email, string password)
        {
            var isSaved = UserService.UpdateUserPassword(email, password);
            if (!isSaved)
            {
                return InvalidUserEmailMessage;
            }
            return "Change saved";
        }

        private static string GetUserImages(string email)
        {
            try
            {
                return UserService.GetUserImagesAsync(email).Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string GetImageComments(string userEmail, string imageName)
        {
            try
            {
                return UserService.GetImageCommentsAsync(userEmail, imageName).Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
