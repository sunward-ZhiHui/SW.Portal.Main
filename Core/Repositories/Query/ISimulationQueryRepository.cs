using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISimulationQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<NavMethodCodeModel>> GetAllNavMethodCodeAsync();
        Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationMidMonth(DateRangeModel dateRangeModel);
        Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV3(DateRangeModel dateRangeModel);
        Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV4(DateRangeModel dateRangeModel);
        Task<IReadOnlyList<INPCalendarPivotModel>> GetSimulationAddhocV5(DateRangeModel dateRangeModel);
        Task<SimulationTicketCalculation> InsertOrUpdateSimulationTicketCalculation(List<SimulationTicketCalculation> value, SimulationTicketCalculationChild simulationTicketCalculationChild);
        Task<IReadOnlyList<SimulationTicketCalculation>> GetAllSimulationTicketCalculation();
        Task<IReadOnlyList<SimulationTicketCalculation>> GetSimulationTicketCalculationVesionNo();
        SimulationTicketCalculation GetSimulationTicketCalculationVersionNoExitsCheckValidation(string? VesionNo);
        Task<IReadOnlyList<SimulationTicketCalculation>> GetSimulationTicketCalculationNotChangesVesionNo(string? VesionNo);
        Task<SimulationTicketCalculation> InsertOrUpdateSimulationTicketCalculationNoVersionChanges(List<SimulationTicketCalculation> simulationTicketCalculations, SimulationTicketCalculation?  simulationTicketCalculation);
    }
}
