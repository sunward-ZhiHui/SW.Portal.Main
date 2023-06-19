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
    public class GetAllConsumptionEntryHandler : IRequestHandler<GetAllConsumptionEntry, List<ViewConsumptionEntry>>
    {
        private readonly IConsumptionQueryRepository _consumptionqueryRepository;
        // private readonly IQueryRepository<ViewProductionEntry> _queryRepository;
        public GetAllConsumptionEntryHandler(IConsumptionQueryRepository consumptionqueryRepository)
        {
            _consumptionqueryRepository = consumptionqueryRepository;
        }
        public async Task<List<ViewConsumptionEntry>> Handle(GetAllConsumptionEntry request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<ViewConsumptionEntry>)await _consumptionqueryRepository.GetAllAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
    public class GetAllConsumptionLinesHandler : IRequestHandler<GetAllConsumptionLinesQuery, List<ViewConsumptionLines>>
    {
        private readonly IConsumptionQueryRepository _consumptionqueryRepository;
        // private readonly IQueryRepository<ViewProductionEntry> _queryRepository;
        public GetAllConsumptionLinesHandler(IConsumptionQueryRepository consumptionqueryRepository)
        {
            _consumptionqueryRepository = consumptionqueryRepository;
        }
        public async Task<List<ViewConsumptionLines>> Handle(GetAllConsumptionLinesQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<ViewConsumptionLines>)await _consumptionqueryRepository.GetAllLineAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
}

