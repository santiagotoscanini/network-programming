using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DataAccess;
using NetworkCommunication;
using Services;

namespace Server
{
    class Server
    {
        private static Dictionary<string, Communication> _clients = new Dictionary<string, Communication>();
        private static UserService _userService = new UserService(new UserRepository());

        private static SessionService
            _sessionService =
                new SessionService(new SessionRepository(),
                    _userService); // cambiar para implementar una inyeccion de dependencia y un singleton

        private static string _commandsToUser = @"'get users' -> to get the register users
'post logout <<your email>>' -> to close your session and close the connection
'put image <<email>> <<path>' -> to save a new image";

        private static string _commandsToServer = @"'get connectedUsers' -> to get the connected users
'put user <<email>> <<password>>' -> to create a new user
'delete user <<email>>' -> to delete a user";

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
            while (true)
            {
                var request = Console.ReadLine(); 
                Console.WriteLine(ExecuteServerRequest(request));
            }
        }
        
        private static string ExecuteUserRequest(string message)
        {
            var words = message.Split(" ");
            var verb = words[0];
            string element;
            
            switch (verb)
            {
                case "help":
                    return _commandsToUser;
                case "get":
                    element = words[1];
                    switch (element)
                    {
                        case "users":
                            return _userService.GetUsers();
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
                    }
                    break;
            }

            return "Bad request";
        }
        
        private static string ExecuteServerRequest(string message)
        {
            var words = message.Split(" ");
            var verb = words[0];
            string element;
            
            switch (verb)
            {
                case "help":
                    return _commandsToServer;
                case "get":
                    element = words[1];
                    switch (element)
                    {
                        case "connectedUsers":
                            return _sessionService.GetLoggedUsers();
                    }

                    break;
                case "put":
                    element = words[1];
                    switch (element)
                    {
                        case "user":
                            var email = words[2]; //deberiamos poner un catch para estas cosas, hacer como en delete
                            var password = words[3];
                            _userService.CreateUser(email, password);
                            return "Created";
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
            }

            return "Bad request";
        }

        private static void WaitingForClients(TcpListener server)
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                var communication = new Communication(stream);
                new Thread(() => StartingUserInteraction(communication)).Start();
            }
        }

        private static void StartingUserInteraction(Communication communication)
        {
            Write(communication, "Welcome, if you want to login in your account write: 'login' or if you want to register write: 'register'");
            string readInfo = ReadInfo(communication);
            switch (readInfo) // ver si estas llamadas recursivas no es mejor meterlas en un while porque un ser malvado puede llenar el stack
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

            Write(communication, "You are connected to InstaFoto, write 'help' to know what commands you can execute. Enjoy!");
            Read(communication);
        }

        private static string Login(Communication communication)
        {
            Write(communication, "Write your email:");
            string userEmail = ReadInfo(communication);
            Write(communication, "Write your password:");
            string userPassword = ReadInfo(communication);
            try
            {
                _sessionService.LoginUser(userEmail, userPassword);
                _clients.Add(userEmail, communication);
            }
            catch (Exception e)
            {
                Write(communication, e.Message);
                return Login(communication); //hacer esto es legal?
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
                _userService.CreateUser(userEmail, userPassword);
                _sessionService.LoginUser(userEmail, userPassword);
                _clients.Add(userEmail, communication);
            }
            catch (Exception e)
            {
                Write(communication, e.Message);
                return Register(communication); //hacer esto es legal?
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
                catch (System.IO.IOException e)
                {
                    return;
                }
            }
        }

        private static string Logout(string email)
        {
            try
            {
                _sessionService.LogoutUser(email);
                _clients.Remove(email); //deberiamos cerrar el hilo en este momento?
                return "You session was closed and you are disconnected";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string DeleteUser(string email)
        {
            try
            {
                _userService.DeleteUser(email);
                return "User deleted";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string ReceiveFile(string email)
        {
            _clients.GetValueOrDefault(email).ReceiveFile();
            return "Successfully saved";
        }

    }
}