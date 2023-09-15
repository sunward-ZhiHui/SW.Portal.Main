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
    public class GetAllAttributeDetailsHandler : IRequestHandler<GetAllAttributeDetailsQuery, List<AttributeDetails>>
    {
        private readonly IQueryRepository<AttributeDetails> _queryRepository;
        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public GetAllAttributeDetailsHandler(IQueryRepository<AttributeDetails> queryRepository, IAttributeDetailsQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetAllAttributeDetailsQuery request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _queryRepository.GetListAsync();
        }
    }

    public class CreateAttributeDetailsHandler : IRequestHandler<CreateAttributeDetails, long>
    {

        private readonly IQueryRepository<AttributeDetails> _queryRepository;
        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public CreateAttributeDetailsHandler(IQueryRepository<AttributeDetails> queryRepository, IAttributeDetailsQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<long> Handle(CreateAttributeDetails request, CancellationToken cancellationToken)
        {
            return (long)await _attrubutequeryRepository.Insert(request);

        }
    }
    public class EditAttributeDetailsHandler : IRequestHandler<EditAttributeDetails, long>
    {
        private readonly IAttributeDetailsQueryRepository _conversationQueryRepository;

        public EditAttributeDetailsHandler(IAttributeDetailsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditAttributeDetails request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.UpdateAsync(request);
            return req;
        }
    }
}

