using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetApplicationPermissionListHandler : IRequestHandler<GetApplicationPermissionListQuery, List<ApplicationPermission>>
    {
        private readonly IApplicationPermissionListQueryRepository _rolepermissionQueryRepository;
        public GetApplicationPermissionListHandler(IApplicationPermissionListQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }
        public async Task<List<ApplicationPermission>> Handle(GetApplicationPermissionListQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationPermission>)await _rolepermissionQueryRepository.GetAllAsync();
        } 
    }

    public class CreateApplicationPermissionListHandler : IRequestHandler<CreateApplicationPermissionListQuery, long>
    {
        private readonly IApplicationPermissionListQueryRepository _rolepermissionQueryRepository;
        public CreateApplicationPermissionListHandler(IApplicationPermissionListQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(CreateApplicationPermissionListQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _rolepermissionQueryRepository.Insert(request);
            return newlist;

        }

    }
    public class EditApplicationPermissionListHandler : IRequestHandler<EditApplicationPermissionListQuery, long>
    {
        private readonly IApplicationPermissionListQueryRepository _rolepermissionQueryRepository;
        public EditApplicationPermissionListHandler(IApplicationPermissionListQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(EditApplicationPermissionListQuery request, CancellationToken cancellationToken)
        {
            var req = await _rolepermissionQueryRepository.Update(request);
            return req;
        }
    }

    public class DeleteApplicationPermissionListHandler : IRequestHandler<DeleteApplicationPermissionListQuery, long>
    {
        private readonly IApplicationPermissionListQueryRepository _rolepermissionQueryRepository;

        public DeleteApplicationPermissionListHandler(IApplicationPermissionListQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(DeleteApplicationPermissionListQuery request, CancellationToken cancellationToken)
        {
            var req = await _rolepermissionQueryRepository.Delete(request.ID,request.permissionid);
            return req;
        }
    }

}
