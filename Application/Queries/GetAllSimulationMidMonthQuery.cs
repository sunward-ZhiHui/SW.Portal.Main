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
    public class GetAllSimulationTicketCalculation : PagedRequest, IRequest<List<SimulationTicketCalculation>>
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
    public class GetSimulationAddhocV4Query : PagedRequest, IRequest<List<INPCalendarPivotModel>>
    {
        public DateRangeModel DateRangeModel { get; set; }
        public GetSimulationAddhocV4Query(DateRangeModel dateRangeModel)
        {
            this.DateRangeModel = dateRangeModel;
        }
    }
    public class GetSimulationAddhocV5Query : PagedRequest, IRequest<List<INPCalendarPivotModel>>
    {
        public DateRangeModel DateRangeModel { get; set; }
        public GetSimulationAddhocV5Query(DateRangeModel dateRangeModel)
        {
            this.DateRangeModel = dateRangeModel;
        }
    }
    public class InsertOrUpdateSimulationTicketCalculation : SimulationTicketCalculation, IRequest<SimulationTicketCalculation>
    {
        public List<SimulationTicketCalculation> SimulationTicketCalculation { get; private set; }
        public SimulationTicketCalculationChild SimulationTicketCalculationChild { get; private set; }
        public InsertOrUpdateSimulationTicketCalculation(List<SimulationTicketCalculation> simulationTicketCalculations, SimulationTicketCalculationChild simulationTicketCalculationChild)
        {
            this.SimulationTicketCalculation = simulationTicketCalculations;
            this.SimulationTicketCalculationChild = simulationTicketCalculationChild;
        }
    }
    public class InsertOrUpdateSimulationTicketCalculationNoVersionChanges : SimulationTicketCalculation, IRequest<SimulationTicketCalculation>
    {
        public List<SimulationTicketCalculation> SimulationTicketCalculation { get; private set; }
        public SimulationTicketCalculation? SimulationTicketCalculationOne { get; private set; }
        public InsertOrUpdateSimulationTicketCalculationNoVersionChanges(List<SimulationTicketCalculation> simulationTicketCalculations, SimulationTicketCalculation? simulationTicketCalculation)
        {
            this.SimulationTicketCalculation = simulationTicketCalculations;
            this.SimulationTicketCalculationOne = simulationTicketCalculation;
        }
    }
    public class GetSimulationTicketCalculationVesionNo : PagedRequest, IRequest<List<SimulationTicketCalculation>>
    {
       
    }
    public class GetSimulationTicketCalculationNotChangesVesionNo : PagedRequest, IRequest<List<SimulationTicketCalculation>>
    {
        public string? VersionNo { get; private set; }
        public GetSimulationTicketCalculationNotChangesVesionNo(string? versionNo)
        {
            this.VersionNo = versionNo;
        }
    }


}
