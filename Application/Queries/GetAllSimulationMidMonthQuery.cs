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
}
