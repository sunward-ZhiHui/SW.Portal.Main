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
        private readonly IQueryRepository<Ictmaster> _queryRepository;
        public GetAllIctmasterHandler(IIctmasterQueryRepository plantQueryRepository, IQueryRepository<Ictmaster>  queryRepository)
        {
            _ictmasterQueryRepository = plantQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewIctmaster>> Handle(GetAllIctMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewIctmaster>)await _ictmasterQueryRepository.GetAllAsync();
        }
    }
    public class GetAllCodeMasterHandler : IRequestHandler<GetAllMasterTypeQuery, List<CodeMaster>>
    {
        private readonly IIctmasterQueryRepository _roleQueryRepository;
        private readonly IQueryRepository<CodeMaster> _queryRepository;
        public GetAllCodeMasterHandler(IIctmasterQueryRepository roleQueryRepository, IQueryRepository<CodeMaster> queryRepository)
        {
            _roleQueryRepository = roleQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<CodeMaster>> Handle(GetAllMasterTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<CodeMaster>)await _roleQueryRepository.GetCodeMasterByStatus(request.MasterType);

        }
    }

}
