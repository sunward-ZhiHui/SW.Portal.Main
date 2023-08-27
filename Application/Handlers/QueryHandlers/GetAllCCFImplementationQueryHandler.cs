using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllCCFImplementationQueryHandler : IRequestHandler<GetAllImplementationQuery, List<CCFDImplementation>>
    {

        private readonly ICCDFImplementationQueryRepository _queryRepository;
        public GetAllCCFImplementationQueryHandler(ICCDFImplementationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<CCFDImplementation>> Handle(GetAllImplementationQuery request, CancellationToken cancellationToken)
        {
            return (List<CCFDImplementation>)await _queryRepository.GetListAsync();
        }
    }
   
}
