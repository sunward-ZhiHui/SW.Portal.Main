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
    public interface IPlanningForProductionProcessByMachineQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<PlanningForProductionProcessByMachine>> GetAllByAsync();
        Task<PlanningForProductionProcessByMachine> DeletePlanningForProductionProcessByMachine(PlanningForProductionProcessByMachine value);
        Task<PlanningForProductionProcessByMachine> InsertOrUpdatePlanningForProductionProcessByMachine(PlanningForProductionProcessByMachine value);
        Task<PlanningForProductionProcessByMachine> GetPlanningForProductionProcessByMachineSession(Guid? SessionId);
        Task<IReadOnlyList<PlanningForProductionProcessByMachineRelated>> GetAllPlanningForProductionProcessByMachineRelatedAsync(long? PlanningForProductionProcessByMachineId);
        Task<PlanningForProductionProcessByMachineRelated> InsertOrUpdatePlanningForProductionProcessByMachineRelated(PlanningForProductionProcessByMachineRelated value);
        Task<PlanningForProductionProcessByMachineRelated> DeletePlanningForProductionProcessByMachineRelated(PlanningForProductionProcessByMachineRelated value);
        Task<IReadOnlyList<ResourceData>> GetSchedulerResourceData();
    }
}
