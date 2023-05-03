using Application.Queries;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllDesignationHandler : IRequestHandler<GetAllDesignationQuery, List<ViewDesignation>>
    {
        private readonly IDesignationQueryRepository _queryRepository;
        public GetAllDesignationHandler(IDesignationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewDesignation>> Handle(GetAllDesignationQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewDesignation>)await _queryRepository.GetAllAsync();
        }
    }
}
