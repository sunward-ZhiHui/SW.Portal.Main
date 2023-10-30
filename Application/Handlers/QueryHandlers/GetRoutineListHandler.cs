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
  
    public class GetRoutineListHandler : IRequestHandler<GetAllRoutineLineList, List<ProductionActivityRoutineAppLine>>
    {
        private readonly IRoutineQueryRepository _productionactivityQueryRepository;
      
        public GetRoutineListHandler(IRoutineQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityRoutineAppLine>> Handle(GetAllRoutineLineList request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityRoutineAppLine>)await _productionactivityQueryRepository.GetAllAsync();
        }


    }
}
