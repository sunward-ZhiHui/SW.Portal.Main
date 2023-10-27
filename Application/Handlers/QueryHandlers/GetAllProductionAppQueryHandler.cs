using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionAppQueryHandler : IRequestHandler<GetAllProductionActivityAppQuery, List<ProductionActivityApp>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetAllProductionAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivityAppQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityApp>)await _productionactivityappQueryRepository.GetAllAsync(request.companyID);
        }

    }

    public class GetAllProductionAppQueryListHandler : IRequestHandler<GetAllProductionActivityAppQueryList, List<ProductionActivityApp>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetAllProductionAppQueryListHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivityAppQueryList request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityApp>)await _productionactivityappQueryRepository.GetAllListAsync();
        }

    }
    public class CreateProductionAppQueryHandler : IRequestHandler<CreateProductionActivityAppCommand, long>
    {
        private readonly IProductionActivityAppQueryRepository _PPAppLineListQueryRepository;
        public CreateProductionAppQueryHandler(IProductionActivityAppQueryRepository PPAppLineListQueryRepository, IQueryRepository<ProductionActivityModel> queryRepository)
        {
            _PPAppLineListQueryRepository = PPAppLineListQueryRepository;
        }

        public async Task<long> Handle(CreateProductionActivityAppCommand request, CancellationToken cancellationToken)
        {
            var newlist = await _PPAppLineListQueryRepository.Insert(request);
            return newlist;

        }
    }
        public class GetAllProductionActivityPONumberAppQueryHandler : IRequestHandler<GetAllProductionActivityPONumberAppQuery, List<ProductionActivityApp>>
        {
            private readonly IProductionActivityAppQueryRepository _productionactivityappponumberQueryRepository;
            public GetAllProductionActivityPONumberAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappponumberQueryRepository)
            {
                _productionactivityappponumberQueryRepository = productionactivityappponumberQueryRepository;
            }
            public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivityPONumberAppQuery request, CancellationToken cancellationToken)
            {
                return (List<ProductionActivityApp>)await _productionactivityappponumberQueryRepository.GetAllAsyncPO(request.companyID);
            }

        }
 }

