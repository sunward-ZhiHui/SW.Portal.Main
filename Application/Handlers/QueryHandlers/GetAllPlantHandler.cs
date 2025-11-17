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
    public class GetAllPlantHandler : IRequestHandler<GetAllPlantQuery, List<ViewPlants>>
    {
        private readonly IPlantQueryRepository _plantQueryRepository;
        public GetAllPlantHandler(IPlantQueryRepository plantQueryRepository)
        {
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<List<ViewPlants>> Handle(GetAllPlantQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewPlants>)await _plantQueryRepository.GetAllAsync();
        }

    }
    public class GetAllPlantByNavCompanyHandler : IRequestHandler<GetAllPlantByNavCompanyQuery, List<ViewPlants>>
    {
        private readonly IPlantQueryRepository _plantQueryRepository;
        public GetAllPlantByNavCompanyHandler(IPlantQueryRepository plantQueryRepository)
        {
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<List<ViewPlants>> Handle(GetAllPlantByNavCompanyQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewPlants>)await _plantQueryRepository.GetAllByNavCompanyAsync();
        }
    }
    public class GetHRMasterAuditListHandler : IRequestHandler<GetHRMasterAuditList, List<HRMasterAuditTrail>>
    {
        private readonly IHRMasterAuditTrailQueryRepository _plantQueryRepository;
        public GetHRMasterAuditListHandler(IHRMasterAuditTrailQueryRepository plantQueryRepository)
        {
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<List<HRMasterAuditTrail>> Handle(GetHRMasterAuditList request, CancellationToken cancellationToken)
        {
            return (List<HRMasterAuditTrail>)await _plantQueryRepository.GetHRMasterAuditList(request.MasterType, request.MasterId, request.IsDeleted, request.SessionId,request.AddTypeId);
        }

    }
    public class GetHRMasterSWAuditListHandler : IRequestHandler<GetHRMasterSWAuditList, List<FileProfileTypeModel>>
    {
        private readonly IHRMasterAuditTrailQueryRepository _plantQueryRepository;
        public GetHRMasterSWAuditListHandler(IHRMasterAuditTrailQueryRepository plantQueryRepository)
        {
            _plantQueryRepository = plantQueryRepository;
        }
        public async Task<List<FileProfileTypeModel>> Handle(GetHRMasterSWAuditList request, CancellationToken cancellationToken)
        {
            return (List<FileProfileTypeModel>)await _plantQueryRepository.GetHRMasterSWAuditList(request.MasterType, request.IsDeleted);
        }

    }
}
