using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllApplicationMasterParenAlltHandler : IRequestHandler<GetAllApplicationMasterParentAllQuery, List<ApplicationMasterParent>>
    {
        private readonly IApplicationMasterParentQueryRepository _queryRepository;
        public GetAllApplicationMasterParenAlltHandler(IApplicationMasterParentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterParent>> Handle(GetAllApplicationMasterParentAllQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterParent>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetAllApplicationMasterParentHandler : IRequestHandler<GetAllApplicationMasterParentQuery, List<ApplicationMasterParent>>
    {
        private readonly IApplicationMasterParentQueryRepository _queryRepository;
        public GetAllApplicationMasterParentHandler(IApplicationMasterParentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterParent>> Handle(GetAllApplicationMasterParentQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterParent>)await _queryRepository.GetAllByParentAsync();
        }
    }
    public class InsertApplicationMasterParentHandler : IRequestHandler<InsertApplicationMasterParent, ApplicationMasterParent>
    {
        private readonly IApplicationMasterParentQueryRepository _queryRepository;
        public InsertApplicationMasterParentHandler(IApplicationMasterParentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ApplicationMasterParent> Handle(InsertApplicationMasterParent request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertApplicationMasterParent(request);
        }
    }
}
