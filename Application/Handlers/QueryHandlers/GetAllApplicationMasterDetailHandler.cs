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

    public class GetAllApplicationMasterresultHandler : IRequestHandler<GetAllApplicationMasterresultQuery, List<ApplicationMasterDetail>>
    {
        private readonly IApplicationMasterDetailQueryRepository _productionactivityQueryRepository;
        public GetAllApplicationMasterresultHandler(IApplicationMasterDetailQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<List<ApplicationMasterDetail>> Handle(GetAllApplicationMasterresultQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterDetail>)await _productionactivityQueryRepository.GetAllAsync();
        }


    }
    public class InsertApplicationMasterHandler : IRequestHandler<InsertApplicationMaster, ApplicationMaster>
    {
        private readonly IApplicationMasterDetailQueryRepository _productionactivityQueryRepository;
        public InsertApplicationMasterHandler(IApplicationMasterDetailQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ApplicationMaster> Handle(InsertApplicationMaster request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.InsertApplicationMaster(request);
        }


    }

    public class GetApplicationMasterAccessSecurityListHandler : IRequestHandler<GetApplicationMasterAccessSecurityList, List<ApplicationMasterAccess>>
    {
        private readonly IApplicationMasterDetailQueryRepository _masterQueryRepository;
        private readonly IQueryRepository<ApplicationMasterAccess> _queryRepository;
        public GetApplicationMasterAccessSecurityListHandler(IApplicationMasterDetailQueryRepository roleQueryRepository, IQueryRepository<ApplicationMasterAccess> queryRepository)
        {
            _masterQueryRepository = roleQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterAccess>> Handle(GetApplicationMasterAccessSecurityList request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterAccess>)await _masterQueryRepository.GetApplicationMasterAccessSecurityList(request.Id,request.AccessTypeFrom);

        }
    }
    public class InsertApplicationMasterAccessSecurityHandler : IRequestHandler<InsertApplicationMasterAccessSecurity, ApplicationMasterAccess>
    {
        private readonly IApplicationMasterDetailQueryRepository _productionactivityQueryRepository;
        public InsertApplicationMasterAccessSecurityHandler(IApplicationMasterDetailQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<ApplicationMasterAccess> Handle(InsertApplicationMasterAccessSecurity request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.InsertApplicationMasterAccessSecurity(request.ApplicationMasterAccess);
        }


    }
    public class DeleteApplicationMasterAccessHandler : IRequestHandler<DeleteApplicationMasterAccess, long?>
    {
        private readonly IApplicationMasterDetailQueryRepository _productionactivityQueryRepository;
        public DeleteApplicationMasterAccessHandler(IApplicationMasterDetailQueryRepository productionactivityQueryRepository)
        {
            _productionactivityQueryRepository = productionactivityQueryRepository;
        }
        public async Task<long?> Handle(DeleteApplicationMasterAccess request, CancellationToken cancellationToken)
        {
            return await _productionactivityQueryRepository.DeleteApplicationMasterAccess(request.Id, request.Ids);
        }


    }
    public class GetApplicationMasterAccessSecurityByMasterHandler : IRequestHandler<GetApplicationMasterAccessSecurityByMaster, List<ApplicationMasterAccess>>
    {
        private readonly IApplicationMasterDetailQueryRepository _masterQueryRepository;
        private readonly IQueryRepository<ApplicationMasterAccess> _queryRepository;
        public GetApplicationMasterAccessSecurityByMasterHandler(IApplicationMasterDetailQueryRepository roleQueryRepository, IQueryRepository<ApplicationMasterAccess> queryRepository)
        {
            _masterQueryRepository = roleQueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterAccess>> Handle(GetApplicationMasterAccessSecurityByMaster request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterAccess>)await _masterQueryRepository.GetApplicationMasterAccessSecurityEmptyAsync(request.Id, request.AccessTypeFrom);

        }
    }

}
