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
    public class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery, List<ViewEmployee>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllEmployeeHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _queryRepository.GetAllAsync();
        }
    }
}
