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
    public class GetAllPlanningForProductionProcessByMachineHandler : IRequestHandler<GetAllPlanningForProductionProcessByMachineQuery, List<PlanningForProductionProcessByMachine>>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public GetAllPlanningForProductionProcessByMachineHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<PlanningForProductionProcessByMachine>> Handle(GetAllPlanningForProductionProcessByMachineQuery request, CancellationToken cancellationToken)
        {
            return (List<PlanningForProductionProcessByMachine>)await _queryRepository.GetAllByAsync();
        }
    }
    
         public class GetPlanningForProductionProcessByMachineBySessionHandler : IRequestHandler<GetPlanningForProductionProcessByMachineBySession, PlanningForProductionProcessByMachine>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public GetPlanningForProductionProcessByMachineBySessionHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<PlanningForProductionProcessByMachine> Handle(GetPlanningForProductionProcessByMachineBySession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetPlanningForProductionProcessByMachineSession(request.SesionId);
        }
    }
    public class InsertOrUpdatePlanningForProductionProcessByMachineHandler : IRequestHandler<InsertOrUpdatePlanningForProductionProcessByMachine, PlanningForProductionProcessByMachine>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public InsertOrUpdatePlanningForProductionProcessByMachineHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<PlanningForProductionProcessByMachine> Handle(InsertOrUpdatePlanningForProductionProcessByMachine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdatePlanningForProductionProcessByMachine(request);

        }
    }

    public class DeletePlanningForProductionProcessByMachineHandler : IRequestHandler<DeletePlanningForProductionProcessByMachine, PlanningForProductionProcessByMachine>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public DeletePlanningForProductionProcessByMachineHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<PlanningForProductionProcessByMachine> Handle(DeletePlanningForProductionProcessByMachine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeletePlanningForProductionProcessByMachine(request.PlanningForProductionProcessByMachine);
        }

    }



    public class GetAllPlanningForProductionProcessByMachineRelatedHandler : IRequestHandler<GetAllPlanningForProductionProcessByMachineRelatedQuery, List<PlanningForProductionProcessByMachineRelated>>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public GetAllPlanningForProductionProcessByMachineRelatedHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<PlanningForProductionProcessByMachineRelated>> Handle(GetAllPlanningForProductionProcessByMachineRelatedQuery request, CancellationToken cancellationToken)
        {
            return (List<PlanningForProductionProcessByMachineRelated>)await _queryRepository.GetAllPlanningForProductionProcessByMachineRelatedAsync(request.PlanningForProductionProcessByMachineId);
        }
    }
    public class DeletePlanningForProductionProcessByMachineRelatedHandler : IRequestHandler<DeletePlanningForProductionProcessByMachineRelated, PlanningForProductionProcessByMachineRelated>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public DeletePlanningForProductionProcessByMachineRelatedHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<PlanningForProductionProcessByMachineRelated> Handle(DeletePlanningForProductionProcessByMachineRelated request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeletePlanningForProductionProcessByMachineRelated(request.PlanningForProductionProcessByMachineRelated);
        }

    }
    public class InsertOrUpdatePlanningForProductionProcessByMachineRelatedHandler : IRequestHandler<InsertOrUpdatePlanningForProductionProcessByMachineRelated, PlanningForProductionProcessByMachineRelated>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public InsertOrUpdatePlanningForProductionProcessByMachineRelatedHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<PlanningForProductionProcessByMachineRelated> Handle(InsertOrUpdatePlanningForProductionProcessByMachineRelated request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdatePlanningForProductionProcessByMachineRelated(request);

        }
    }
    public class GetSchedulerResourceDataHandler : IRequestHandler<GetSchedulerResourceData, List<ResourceData>>
    {
        private readonly IPlanningForProductionProcessByMachineQueryRepository _queryRepository;
        public GetSchedulerResourceDataHandler(IPlanningForProductionProcessByMachineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ResourceData>> Handle(GetSchedulerResourceData request, CancellationToken cancellationToken)
        {
            return (List<ResourceData>)await _queryRepository.GetSchedulerResourceData();
        }
    }
}
