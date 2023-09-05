using Application.Queries;
using Core.Entities;
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
        public GetAllAttributeHandler(IQueryRepository<AttributeHeader> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<AttributeHeader>> Handle(GetAllAttributeHeader request, CancellationToken cancellationToken)
        {
            return (List<AttributeHeader>)await _queryRepository.GetListAsync();
        }
    }

    public class CreateAttributeHandler : IRequestHandler<CreateAttributeHeader, AttributeHeader>
    {

        private readonly IQueryRepository<AttributeHeader> _queryRepository;
        public CreateAttributeHandler(IQueryRepository<AttributeHeader> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<AttributeHeader> Handle(CreateAttributeHeader request, CancellationToken cancellationToken)
        {
            return (AttributeHeader)_queryRepository.Add(request);
        }
    }
}
