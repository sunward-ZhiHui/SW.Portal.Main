using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetSimulationsHandler : IRequestHandler<GetAllSimulationMidMonthQuery, List<INPCalendarPivotModel>>
    {
        private readonly ISimulationQueryRepository _queryRepository;
        public GetSimulationsHandler(ISimulationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<INPCalendarPivotModel>> Handle(GetAllSimulationMidMonthQuery request, CancellationToken cancellationToken)
        {
            return (List<INPCalendarPivotModel>)await _queryRepository.GetSimulationMidMonth(request.DateRangeModel);
        }
    }
    public class GetAllNavMethodCodeHandler : IRequestHandler<GetAllNavMethodCode, List<NavMethodCodeModel>>
    {
        private readonly ISimulationQueryRepository _queryRepository;
        public GetAllNavMethodCodeHandler(ISimulationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavMethodCodeModel>> Handle(GetAllNavMethodCode request, CancellationToken cancellationToken)
        {
            return (List<NavMethodCodeModel>)await _queryRepository.GetAllNavMethodCodeAsync();
        }
    }
    public class GetSimulationAddhocV3QueryHandler : IRequestHandler<GetSimulationAddhocV3Query, List<INPCalendarPivotModel>>
    {
        private readonly ISimulationQueryRepository _queryRepository;
        public GetSimulationAddhocV3QueryHandler(ISimulationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<INPCalendarPivotModel>> Handle(GetSimulationAddhocV3Query request, CancellationToken cancellationToken)
        {
            return (List<INPCalendarPivotModel>)await _queryRepository.GetSimulationAddhocV3(request.DateRangeModel);
        }
    }
}
