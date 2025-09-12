using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllPlanningForProductionProcessByMachineQuery : PagedRequest, IRequest<List<PlanningForProductionProcessByMachine>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdatePlanningForProductionProcessByMachine : PlanningForProductionProcessByMachine, IRequest<PlanningForProductionProcessByMachine>
    {

    }
    public class DeletePlanningForProductionProcessByMachine : PlanningForProductionProcessByMachine, IRequest<PlanningForProductionProcessByMachine>
    {
        public PlanningForProductionProcessByMachine PlanningForProductionProcessByMachine { get; private set; }
        public DeletePlanningForProductionProcessByMachine(PlanningForProductionProcessByMachine aCEntryModel)
        {
            this.PlanningForProductionProcessByMachine = aCEntryModel;
        }
    }
    public class GetPlanningForProductionProcessByMachineBySession : PagedRequest, IRequest<PlanningForProductionProcessByMachine>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetPlanningForProductionProcessByMachineBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }



    public class GetAllPlanningForProductionProcessByMachineRelatedQuery : PagedRequest, IRequest<List<PlanningForProductionProcessByMachineRelated>>
    {
        public long? PlanningForProductionProcessByMachineId { get; set; }
        public GetAllPlanningForProductionProcessByMachineRelatedQuery(long? planningForProductionProcessByMachineId)
        {
            PlanningForProductionProcessByMachineId = planningForProductionProcessByMachineId;
        }
    }
    public class InsertOrUpdatePlanningForProductionProcessByMachineRelated : PlanningForProductionProcessByMachineRelated, IRequest<PlanningForProductionProcessByMachineRelated>
    {

    }
    public class DeletePlanningForProductionProcessByMachineRelated : PlanningForProductionProcessByMachineRelated, IRequest<PlanningForProductionProcessByMachineRelated>
    {
        public PlanningForProductionProcessByMachineRelated PlanningForProductionProcessByMachineRelated { get; private set; }
        public DeletePlanningForProductionProcessByMachineRelated(PlanningForProductionProcessByMachineRelated aCEntryModel)
        {
            this.PlanningForProductionProcessByMachineRelated = aCEntryModel;
        }
    }
}
