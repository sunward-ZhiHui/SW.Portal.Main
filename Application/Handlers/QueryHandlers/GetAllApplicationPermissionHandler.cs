using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllApplicationPermissionHandler : IRequestHandler<GetAllApplicationPermissionQuery, List<PortalMenuModel>>
    {
        private readonly IMenuPermissionQueryRepository _queryRepository;
        public GetAllApplicationPermissionHandler(IMenuPermissionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<PortalMenuModel>> Handle(GetAllApplicationPermissionQuery request, CancellationToken cancellationToken)
        {
            return (List<PortalMenuModel>)await _queryRepository.GetAllAsync(request.Id);
        }
    }
    public class GetAllApplicationPermissionAllHandler : IRequestHandler<GetAllApplicationPermissionAllQuery, List<PortalMenuModel>>
    {
        private readonly IMenuPermissionQueryRepository _queryRepository;
        public GetAllApplicationPermissionAllHandler(IMenuPermissionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<PortalMenuModel>> Handle(GetAllApplicationPermissionAllQuery request, CancellationToken cancellationToken)
        {
            return (List<PortalMenuModel>)await _queryRepository.GetAllByAsync(request.Id);
        }
    }
    public class GetAllApplicationHandler : IRequestHandler<GetAllApplicationPermission, List<ApplicationPermission>>
    {
        private readonly IApplicationPermissionQueryRepository _queryRepository;
        public GetAllApplicationHandler(IApplicationPermissionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationPermission>> Handle(GetAllApplicationPermission request, CancellationToken cancellationToken)
        {
            return (List<ApplicationPermission>)await _queryRepository.GetAllAsync();
        }
    }
    public class CreateRolePermissionHandler : IRequestHandler<CreateApplicationRolePermissionQuery, long>
    {
        private readonly IApplicationPermissionQueryRepository _rolepermissionQueryRepository;
        public CreateRolePermissionHandler(IApplicationPermissionQueryRepository rolepermissionQueryRepository)
        {
            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }

        public async Task<long> Handle(CreateApplicationRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _rolepermissionQueryRepository.Insert(request);
            return newlist;

        }

    }
}
