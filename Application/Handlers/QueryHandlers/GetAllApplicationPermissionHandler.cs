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
}
