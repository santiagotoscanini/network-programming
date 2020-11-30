using System;
using System.Collections.Generic;
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

        public override Task<AddLoggedUserResponse> AddLoggedUser(AddLoggedUserRequest request, ServerCallContext context)
        {
            _sessionRepository.AddLoggedUser(request.UserEmail);
            return Task.FromResult(new AddLoggedUserResponse {});
        }
    }
}
