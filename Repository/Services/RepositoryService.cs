using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Repository.RepositoriesInterfaces;
using Repository.ServiceInterfaces;

namespace Repository
{
    public class RepositoryService : Repo.RepoBase
    {
        private readonly ILogger<RepositoryService> _logger;
        private readonly ISessionRepository _sessionRepository;
        private ILogSenderService _logSenderService;

        public RepositoryService(ILogger<RepositoryService> logger, ISessionRepository sessionRepository, ILogSenderService logSenderService)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
            _logSenderService = logSenderService;
        }

        public override Task<EmptyMessage> AddLoggedUser(AddLoggedUserRequest request, ServerCallContext context)
        {
            _sessionRepository.AddLoggedUser(request.UserEmail);
            _logSenderService.SendMessages("Log: user " + request.UserEmail + " is logged");
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
            _logSenderService.SendMessages("Log: the logged in users were obtained");
            return Task.FromResult(getLoggedUsersToReturn);
        }

        public override Task<EmptyMessage> DeleteLoggedUser(GetLoggedUser request, ServerCallContext context)
        {
            var loggedUser = _sessionRepository.GetLoggedUsers().Find(u => u.Email.Equals(request.Email));
            _sessionRepository.DeleteLoggedUser(loggedUser);
            _logSenderService.SendMessages("Log: user "+ request.Email+" closed session");
            return Task.FromResult(new EmptyMessage {});
        }
    }
}
