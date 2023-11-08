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
    public class GetAllProductionActivityAppLineHandler : IRequestHandler<GetAllProductionActivityAppLineQuery, List<ProductActivityAppModel>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivityAppLineHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductActivityAppModel>> Handle(GetAllProductionActivityAppLineQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductActivityAppModel>)await _productionactivityQueryRepository.GetAllAsync(request.CompanyID,request.ProdorderNo,request.UserId,request.LocationID);
        }

    }
   
    public class GetAllProductionActivitylocHandler : IRequestHandler<GetAllProductionActivitylocQuery, List<ProductionActivityApp>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivitylocHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivitylocQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityApp>)await _productionactivityQueryRepository.GetAlllocAsync(request.ProductionActivityAppID);
        }

    }

    public class CreateProductionAppLineQueryHandler : IRequestHandler<CreateProductionActivityAppLineCommand, long>
    {
        private readonly IProductionActivityQueryRepository _PPAppLinesListQueryRepository;
        public CreateProductionAppLineQueryHandler(IProductionActivityQueryRepository PPAppLineSListQueryRepository, IQueryRepository<ProductionActivityAppLine> queryRepository)
        {
            _PPAppLinesListQueryRepository = PPAppLineSListQueryRepository;
        }

        public async Task<long> Handle(CreateProductionActivityAppLineCommand request, CancellationToken cancellationToken)
        {
            var newlist = await _PPAppLinesListQueryRepository.Insert(request);
            return newlist;

        }
    }
}