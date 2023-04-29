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
    public class GetAllSubSectionHandler : IRequestHandler<GetAllSubSectionQuery, List<ViewSubSection>>
    {
        private readonly ISubSectionQueryRepository _queryRepository;
        public GetAllSubSectionHandler(ISubSectionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewSubSection>> Handle(GetAllSubSectionQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewSubSection>)await _queryRepository.GetAllAsync();
        }
    }
}
