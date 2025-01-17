using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Repository.RepositoriesInterfaces;
using Repository.Services;

namespace Repository
{
    public class RepositoryService : Repo.RepoBase
    {
        private readonly ISessionRepository _sessionRepository;
        private LogSenderService _logSenderService;

        public RepositoryService(ISessionRepository sessionRepository, LogSenderService logSenderService)
        {
            _sessionRepository = sessionRepository;
            _logSenderService = logSenderService;
        }

        public override Task<EmptyMessage> AddLoggedUser(AddLoggedUserRequest request, ServerCallContext context)
        {
            _sessionRepository.AddLoggedUser(request.UserEmail);
            _logSenderService.SendMessages("user " + request.UserEmail + " is logged");
            return Task.FromResult(new EmptyMessage {});
        }

        public override Task<GetLoggedUsersResponse> GetLoggedUsers(EmptyMessage request, ServerCallContext context)
        {
            var loggedUsers = _sessionRepository.GetLoggedUsers();
            var getLoggedUsers = loggedUsers.Select(u => new GetLoggedUser { 
                Email = u.Email,
                ConnectionDate = u.ConnectionDate.ToString(),
            });
            var getLoggedUsersToReturn = new GetLoggedUsersResponse();
            foreach (GetLoggedUser user in getLoggedUsers) 
            {
                getLoggedUsersToReturn.LoggedUsers.Add(user);
            }
            _logSenderService.SendMessages("the logged in users were obtained");
            return Task.FromResult(getLoggedUsersToReturn);
        }

        public override Task<EmptyMessage> DeleteLoggedUser(GetLoggedUser request, ServerCallContext context)
        {
            var loggedUser = _sessionRepository.GetLoggedUsers().Find(u => u.Email.Equals(request.Email));
            _sessionRepository.DeleteLoggedUser(loggedUser);
            _logSenderService.SendMessages("user " + request.Email + " closed session");
            return Task.FromResult(new EmptyMessage {});
        }
    }
}
