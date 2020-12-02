using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Repository.RepositoriesInterfaces;

namespace Repository
{
    public class RepositoryService : Repo.RepoBase
    {
        private readonly ILogger<RepositoryService> _logger;
        private readonly ISessionRepository _sessionRepository;

        public RepositoryService(ILogger<RepositoryService> logger, ISessionRepository sessionRepository)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
        }

        public override Task<EmptyMessage> AddLoggedUser(AddLoggedUserRequest request, ServerCallContext context)
        {
            _sessionRepository.AddLoggedUser(request.UserEmail);
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
            return Task.FromResult(getLoggedUsersToReturn);
        }

        public override Task<EmptyMessage> DeleteLoggedUser(GetLoggedUser request, ServerCallContext context)
        {
            var loggedUser = _sessionRepository.GetLoggedUsers().Find(u => u.Email.Equals(request.Email));
            _sessionRepository.DeleteLoggedUser(loggedUser);
            return Task.FromResult(new EmptyMessage {});
        }
    }
}
