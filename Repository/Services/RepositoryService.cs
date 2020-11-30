using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Repository.Repositories;

namespace Repository
{
    public class RepositoryService : Repo.RepoBase
    {
        private readonly ILogger<RepositoryService> _logger;
        private readonly SessionRepository _sessionRepository;
        public RepositoryService(ILogger<RepositoryService> logger)
        {
            _logger = logger;
            _sessionRepository = new SessionRepository();
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
    }
}
