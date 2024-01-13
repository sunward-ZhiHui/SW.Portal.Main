using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, List<ApplicationRole>>
    {
        private readonly IRoleQueryRepository _roleQueryRepository;
        private readonly IQueryRepository<ApplicationRole> _queryRepository;
        public GetAllRoleHandler(IRoleQueryRepository roleQueryRepository, IQueryRepository<ApplicationRole> queryRepository)
        {
            _roleQueryRepository = roleQueryRepository;
            _queryRepository= queryRepository;
        }
        public async Task<List<ApplicationRole>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            //return (List<ApplicationRole>)await _queryRepository.GetListAsync();
            return (List<ApplicationRole>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class GetAllUserGroupsQueryHandler : IRequestHandler<GetAllUserGroupsQuery, List<UserGroup>>
    {
        private readonly IUserGroupQueryRepository _userGroupQueryRepository;
        public GetAllUserGroupsQueryHandler(IUserGroupQueryRepository userGroupQueryRepository)
        {
            _userGroupQueryRepository = userGroupQueryRepository;
        }
        public async Task<List<UserGroup>> Handle(GetAllUserGroupsQuery request, CancellationToken cancellationToken)
        {
            return (List<UserGroup>)await _userGroupQueryRepository.GetAllAsync();
        }
    }
    public class InsertOrUpdateUserGroupHandler : IRequestHandler<InsertOrUpdateUserGroup, UserGroup>
    {
        private readonly IUserGroupQueryRepository _userGroupQueryRepository;
        public InsertOrUpdateUserGroupHandler(IUserGroupQueryRepository userGroupQueryRepository)
        {
            _userGroupQueryRepository = userGroupQueryRepository;
        }
        public async Task<UserGroup> Handle(InsertOrUpdateUserGroup request, CancellationToken cancellationToken)
        {
            return await _userGroupQueryRepository.InsertOrUpdateUserGroup(request);
        }

    }
    public class DeleteUserGroupHandler : IRequestHandler<DeleteUserGroup, UserGroup>
    {
        private readonly IUserGroupQueryRepository _userGroupQueryRepository;
        public DeleteUserGroupHandler(IUserGroupQueryRepository userGroupQueryRepository)
        {
            _userGroupQueryRepository = userGroupQueryRepository;
        }
        public async Task<UserGroup> Handle(DeleteUserGroup request, CancellationToken cancellationToken)
        {
            return await _userGroupQueryRepository.DeleteUserGroup(request.UserGroup);
        }

    }
}
