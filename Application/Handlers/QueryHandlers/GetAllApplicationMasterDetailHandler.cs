using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllApplicationMasterDetailHandler : IRequestHandler<GetAllApplicationMasterDetailQuery, List<View_ApplicationMasterDetail>>
    {
        private readonly IApplicationMasterDetailQueryRepository _masterQueryRepository;
        private readonly IQueryRepository<View_ApplicationMasterDetail> _queryRepository;
        public GetAllApplicationMasterDetailHandler(IApplicationMasterDetailQueryRepository roleQueryRepository, IQueryRepository<View_ApplicationMasterDetail> queryRepository)
        {
            _masterQueryRepository = roleQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<View_ApplicationMasterDetail>> Handle(GetAllApplicationMasterDetailQuery request, CancellationToken cancellationToken)
        {
            return (List<View_ApplicationMasterDetail>)await _masterQueryRepository.GetApplicationMasterByCode(request.Id);

        }
    }
    public class GetAllApplicationMasterHandler : IRequestHandler<GetAllApplicationMasterQuery, List<ApplicationMaster>>
    {
        private readonly IQueryRepository<ApplicationMaster> _queryRepository;
        public GetAllApplicationMasterHandler(IQueryRepository<ApplicationMaster> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMaster>> Handle(GetAllApplicationMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMaster>)await _queryRepository.GetListAsync();

        }
    }
}
