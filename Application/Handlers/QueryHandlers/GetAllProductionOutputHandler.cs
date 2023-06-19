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
    public class GetAllProductionOutputHandler: IRequestHandler<GetAllProductionOutputQuery, List<ViewProductionOutput>>
    {
        private readonly IProductionOutputQueryRepository _productionqueryRepository;
        private readonly IQueryRepository<AppSampling> _queryRepository;
        public GetAllProductionOutputHandler(IProductionOutputQueryRepository productionqueryRepository)
        {
            _productionqueryRepository = productionqueryRepository;
        }
        public async Task<List<ViewProductionOutput>> Handle(GetAllProductionOutputQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<ViewProductionOutput>)await _productionqueryRepository.GetAllAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
   
}
