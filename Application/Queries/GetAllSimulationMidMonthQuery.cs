using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using MediatR;


namespace Application.Queries
{
    public class GetAllSimulationMidMonthQuery : PagedRequest, IRequest<List<INPCalendarPivotModel>>
    {
        public DateRangeModel DateRangeModel { get; set; }
        public GetAllSimulationMidMonthQuery(DateRangeModel dateRangeModel)
        {
            this.DateRangeModel = dateRangeModel;
        }
    }
    public class GetAllNavMethodCode : PagedRequest, IRequest<List<NavMethodCodeModel>>
    {

    }
    public class GetSimulationAddhocV3Query : PagedRequest, IRequest<List<INPCalendarPivotModel>>
    {
        public DateRangeModel DateRangeModel { get; set; }
        public GetSimulationAddhocV3Query(DateRangeModel dateRangeModel)
        {
            this.DateRangeModel = dateRangeModel;
        }
    }
}
