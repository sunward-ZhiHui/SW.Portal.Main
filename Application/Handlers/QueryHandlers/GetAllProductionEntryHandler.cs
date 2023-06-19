using Application.Queries;
using Core.Entities;
using Core.Repositories.Query.Base;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Views;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionEntryHandler : IRequestHandler<GetAllProductionEntryQuery, List<ViewProductionEntry>>
    {
        private readonly IProductionEntryQueryRepository _productionqueryRepository;
      // private readonly IQueryRepository<ViewProductionEntry> _queryRepository;
        public GetAllProductionEntryHandler(IProductionEntryQueryRepository productionqueryRepository)
        {
            _productionqueryRepository = productionqueryRepository;
        }
        public async Task<List<ViewProductionEntry>> Handle(GetAllProductionEntryQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<ViewProductionEntry>)await _productionqueryRepository.GetAllAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
   
}
