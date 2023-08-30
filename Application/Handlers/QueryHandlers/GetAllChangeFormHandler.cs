using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
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
    public class GetAllChangeFormHandler : IRequestHandler<GetAllChangeFormQuery, List<ChangeControlForm>>
    {

        private readonly IChangeControlFormQueryRepository _queryRepository;
        public GetAllChangeFormHandler(IChangeControlFormQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ChangeControlForm>> Handle(GetAllChangeFormQuery request, CancellationToken cancellationToken)
        {
            return (List<ChangeControlForm>)await _queryRepository.GetAllAsync();
        }
    }
    public class CreateChangeControlHandler : IRequestHandler<CreateChangeControlQuery, long>
    {
        private readonly IChangeControlFormQueryRepository _QueryRepository;
        public CreateChangeControlHandler(IChangeControlFormQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(CreateChangeControlQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _QueryRepository.Insert(request);
            return newlist;

        }

    }
}
