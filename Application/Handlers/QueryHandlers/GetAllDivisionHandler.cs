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
    public class GetAllDivisionHandler : IRequestHandler<GetAllDivisionQuery, List<ViewDivision>>
    {
        private readonly IDivisionQueryRepository _queryRepository;
        public GetAllDivisionHandler(IDivisionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewDivision>> Handle(GetAllDivisionQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewDivision>)await _queryRepository.GetAllAsync();
        }
    }
}
