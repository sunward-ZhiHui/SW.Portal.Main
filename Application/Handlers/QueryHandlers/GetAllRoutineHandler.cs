using Application.Queries;
using Core.Entities;
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
   

    public class GetAllRoutineAppQueryHandler : IRequestHandler<GetRoutineLineAppQuery, List<ProductionActivityRoutineAppLine>>
    {
        private readonly IRoutineLineAppQuery _routineactivityQueryRepository;
        public GetAllRoutineAppQueryHandler(IRoutineLineAppQuery productionactivityappQueryRepository)
        {
            _routineactivityQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<ProductionActivityRoutineAppLine>> Handle(GetRoutineLineAppQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityRoutineAppLine>)await _routineactivityQueryRepository.GetAllAsync(request.companyID);
        }

    }

    //public class GetRoutineAppLineFilterReportHandler : IRequestHandler<GetAllProductionActivityAppLineFilterReport, List<View_RoutineActivityAppLineReport>>
    //{
    //    private readonly IRoutineQueryRepository _queryRepository;
    //    public GetRoutineAppLineFilterReportHandler(IRoutineQueryRepository queryRepository)
    //    {
    //        _queryRepository = queryRepository;
    //    }
    //    public async Task<List<View_RoutineActivityAppLineReport>> Handle(GetAllProductionActivityAppLineFilterReport request, CancellationToken cancellationToken)
    //    {
    //        return (List<View_RoutineActivityAppLineReport>)await _queryRepository.GetAllFilterAsync(request.CompanyId, request.FromDate, request.ToDate);
    //    }
    //}

}
