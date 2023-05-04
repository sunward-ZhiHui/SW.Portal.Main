using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllCodeMasterHandler : IRequestHandler<GetAllCodeMasterQuery, List<CodeMaster>>
    {
        private readonly ICodeMasterQueryRepository _roleQueryRepository;
        private readonly IQueryRepository<CodeMaster> _queryRepository;
        public GetAllCodeMasterHandler(ICodeMasterQueryRepository roleQueryRepository, IQueryRepository<CodeMaster> queryRepository)
        {
            _roleQueryRepository = roleQueryRepository;
            _queryRepository= queryRepository;
        }
        public async Task<List<CodeMaster>> Handle(GetAllCodeMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<CodeMaster>)await _roleQueryRepository.GetCodeMasterByStatus(request.Name);

        }
    }
    public class GetAllCodeHandler : IRequestHandler<GetAllCodQuery, List<CodeMaster>>
    {
       
        private readonly IQueryRepository<CodeMaster> _queryRepository;
        public GetAllCodeHandler( IQueryRepository<CodeMaster> queryRepository)
        {
           
            _queryRepository = queryRepository;
        }
        public async Task<List<CodeMaster>> Handle(GetAllCodQuery request, CancellationToken cancellationToken)
        {
            return (List<CodeMaster>)await _queryRepository.GetListAsync();

        }
    }
}
