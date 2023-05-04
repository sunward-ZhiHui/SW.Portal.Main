using Application.Queries;
using Core.Entities;
using Core.Repositories.Query.Base;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Views;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllIctmasterHandler : IRequestHandler<GetAllIctMasterQuery, List<ViewIctmaster>>
    {
        private readonly IIctmasterQueryRepository _ictmasterQueryRepository;
        public GetAllIctmasterHandler(IIctmasterQueryRepository plantQueryRepository)
        {
            _ictmasterQueryRepository = plantQueryRepository;
        }
        public async Task<List<ViewIctmaster>> Handle(GetAllIctMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewIctmaster>)await _ictmasterQueryRepository.GetAllAsync();
        }
    }
    
    
}
