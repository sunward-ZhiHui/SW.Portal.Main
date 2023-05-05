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
    public class GetAllLayOutPlanTypeHandler : IRequestHandler<GetAllLayOutPlanTypeQuery, List<ViewLayOutPlanType>>
    {
        private readonly ILayOutPlanTypeQueryRepository _queryRepository;
        public GetAllLayOutPlanTypeHandler(ILayOutPlanTypeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewLayOutPlanType>> Handle(GetAllLayOutPlanTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewLayOutPlanType>)await _queryRepository.GetAllAsync();
        }
    }
    
}
