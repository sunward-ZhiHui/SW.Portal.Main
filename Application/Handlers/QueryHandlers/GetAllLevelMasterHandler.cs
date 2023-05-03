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
    public class GetAllLevelMasterHandler : IRequestHandler<GetAllLevelMasterQuery, List<ViewLevel>>
    {
        private readonly ILevelMasterQueryRepository _queryRepository;
        public GetAllLevelMasterHandler(ILevelMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewLevel>> Handle(GetAllLevelMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewLevel>)await _queryRepository.GetAllAsync();
        }
    }
}
