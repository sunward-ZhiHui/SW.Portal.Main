using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllResetPermissionsHandler : IRequestHandler<GetResetPermissionsUserGroupUsers, List<UserGroupUser>>
    {
        private readonly IResetPermissionsQueryRepository _queryRepository;
        public GetAllResetPermissionsHandler(IResetPermissionsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<UserGroupUser>> Handle(GetResetPermissionsUserGroupUsers request, CancellationToken cancellationToken)
        {
            return (List<UserGroupUser>)await _queryRepository.GetUserGroupUsers(request.UserIds);
        }
    }
}
