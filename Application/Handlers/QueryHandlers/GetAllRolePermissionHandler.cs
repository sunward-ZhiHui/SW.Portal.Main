using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllRolePermissionHandler : IRequestHandler<GetAllRolePermission, List<ApplicationRole>>
    {
        private readonly IRolePermissionQueryRepository _rolepermissionQueryRepository;
        public GetAllRolePermissionHandler(IRolePermissionQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }
        public async Task<List<ApplicationRole>> Handle(GetAllRolePermission request, CancellationToken cancellationToken)
        {
            return (List<ApplicationRole>)await _rolepermissionQueryRepository.GetAllAsync();
        }
    
    }
    public class CreateRolePermissionHandler : IRequestHandler<CreateRolePermissionQuery, long>
    {
        private readonly IRolePermissionQueryRepository _rolepermissionQueryRepository;
        public CreateRolePermissionHandler(IRolePermissionQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(CreateRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _rolepermissionQueryRepository.Insert(request);
            return newlist;

        }

    }
    public class EditRolePermissionHandler : IRequestHandler<EditRolePermissionQuery, long>
    {
        private readonly IRolePermissionQueryRepository _rolepermissionQueryRepository;
        public EditRolePermissionHandler(IRolePermissionQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(EditRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var req = await _rolepermissionQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteRolePermissionHandler : IRequestHandler<DeleteRolePermissionQuery, long>
    {
        private readonly IRolePermissionQueryRepository _rolepermissionQueryRepository;

        public DeleteRolePermissionHandler(IRolePermissionQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(DeleteRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var req = await _rolepermissionQueryRepository.Delete(request.ID);
            return req;
        }
    }

    public class GetSelectedRolePermissionListHandler : IRequestHandler<GetAllRolePermissionSelectedLst, List<ApplicationRole>>
    {
        private readonly IRolePermissionQueryRepository _rolepermissionQueryRepository;

        public GetSelectedRolePermissionListHandler(IRolePermissionQueryRepository rolepermissionQueryRepository)
        {

            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }
        public async Task<List<ApplicationRole>> Handle(GetAllRolePermissionSelectedLst request, CancellationToken cancellationToken)
        {
            return (List<ApplicationRole>)await _rolepermissionQueryRepository.GetSelectedRolePermissionListAsync(request.RoleID);
        }
    }
}
