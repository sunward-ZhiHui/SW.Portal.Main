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
    public class GetAllApplicationMasterChildHandler : IRequestHandler<GetAllApplicationMasterChildQuery, List<ApplicationMasterChildModel>>
    {
        private readonly IApplicationMasterChildQueryRepository _queryRepository;
        public GetAllApplicationMasterChildHandler(IApplicationMasterChildQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterChildModel>> Handle(GetAllApplicationMasterChildQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterChildModel>)await _queryRepository.GetAllAsync(request.SearchString);
        }
    }
    public class GetAllApplicationMasterChildListHandler : IRequestHandler<GetAllApplicationMasterChildListQuery, List<ApplicationMasterChildModel>>
    {
        private readonly IApplicationMasterChildQueryRepository _queryRepository;
        public GetAllApplicationMasterChildListHandler(IApplicationMasterChildQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterChildModel>> Handle(GetAllApplicationMasterChildListQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterChildModel>)await _queryRepository.GetAllByAsync(request.SearchString);
        }
    }
    public class GetAllApplicationMasterChildListByIdHandler : IRequestHandler<GetAllApplicationMasterChildByIdQuery, List<ApplicationMasterChildModel>>
    {
        private readonly IApplicationMasterChildQueryRepository _queryRepository;
        public GetAllApplicationMasterChildListByIdHandler(IApplicationMasterChildQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ApplicationMasterChildModel>> Handle(GetAllApplicationMasterChildByIdQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationMasterChildModel>)await _queryRepository.GetAllByIdAsync(request.Id);
        }
    }
}
