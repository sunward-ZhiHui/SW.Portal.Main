using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllLanguageHandler : IRequestHandler<GetAllLanguageQuery, List<LanguageMaster>>
    {
        private readonly IQueryRepository<LanguageMaster> _queryRepository;
        public GetAllLanguageHandler(IQueryRepository<LanguageMaster> queryRepository)
        {
            _queryRepository= queryRepository;
        }
        public async Task<List<LanguageMaster>> Handle(GetAllLanguageQuery request, CancellationToken cancellationToken)
        {
            return (List<LanguageMaster>)await _queryRepository.GetListAsync();
        }
    }
}
