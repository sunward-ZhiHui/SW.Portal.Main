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
    public class GetAllAppPermissionHandler : IRequestHandler<GetAllAppPermissionQuery, List<AppPermissionModel>>
    {
        private readonly IAppPermissionQueryRepository _queryRepository;
        public GetAllAppPermissionHandler(IAppPermissionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<AppPermissionModel>> Handle(GetAllAppPermissionQuery request, CancellationToken cancellationToken)
        {
            return (List<AppPermissionModel>)await _queryRepository.GetAllAsync();
        }
    }
}
