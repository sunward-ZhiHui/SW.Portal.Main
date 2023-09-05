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
    public class GetAllAttributeHandler : IRequestHandler<GetAllAttributeHeader, List<AttributeHeader>>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public GetAllAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository=attrubutequeryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeHeader request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _queryRepository.GetListAsync();
        }
    }

    public class CreateAttributeHandler : IRequestHandler<CreateAttributeHeader, long>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        private readonly IAttributeQueryRepository _attrubutequeryRepository;
        public CreateAttributeHandler(IQueryRepository<AttributeHeader> queryRepository, IAttributeQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<long> Handle(CreateAttributeHeader request, CancellationToken cancellationToken)
        {
            return (long) await _attrubutequeryRepository.Insert(request);
        }
    }
}
