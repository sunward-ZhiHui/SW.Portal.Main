using Application.Queries;
using Core.Entities;
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
    public class GetAllProductionActivityAppLineHandler : IRequestHandler<GetAllProductionActivityAppLineQuery, List<ProductionActivityAppLine>>
    {
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        public GetAllProductionActivityAppLineHandler(IProductionActivityQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ProductionActivityAppLine>> Handle(GetAllProductionActivityAppLineQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityAppLine>)await _productionactivityQueryRepository.GetAllAsync();
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