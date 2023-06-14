using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllAssetCatalogMasterHandler : IRequestHandler<GetAllAssetCatalogMasterQuery, List<View_AssetCatalogMaster>>
    {
        private readonly IAssetCatalogMasterQueryRepository _queryRepository;
        public GetAllAssetCatalogMasterHandler(IAssetCatalogMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_AssetCatalogMaster>> Handle(GetAllAssetCatalogMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<View_AssetCatalogMaster>)await _queryRepository.GetAllAsync(request.CategoryId, request.SubSectionId);
        }
    }
    public class GetAllAssetPartsMaintenaceMasterHandler : IRequestHandler<GetAllAssetPartsMaintenaceMasterQuery, List<View_AssetPartsMaintenaceMaster>>
    {
        private readonly IAssetPartsMaintenaceMasterQueryRepository _queryRepository;
        public GetAllAssetPartsMaintenaceMasterHandler(IAssetPartsMaintenaceMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_AssetPartsMaintenaceMaster>> Handle(GetAllAssetPartsMaintenaceMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<View_AssetPartsMaintenaceMaster>)await _queryRepository.GetAllAsync(request.AssetCatalogMasterId);
        }
    }
    public class GetAllAssetEquipmentMaintenaceMasterHandler : IRequestHandler<GetAllAssetEquipmentMaintenaceMasterQuery, List<View_AssetEquipmentMaintenaceMaster>>
    {
        private readonly IAssetEquipmentMaintenaceMasterQueryRepository _queryRepository;
        public GetAllAssetEquipmentMaintenaceMasterHandler(IAssetEquipmentMaintenaceMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_AssetEquipmentMaintenaceMaster>> Handle(GetAllAssetEquipmentMaintenaceMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<View_AssetEquipmentMaintenaceMaster>)await _queryRepository.GetAllAsync(request.AssetPartsMaintenaceMasterId);
        }
    }
    public class GetAllAssetEquipmentMaintenaceMasterAssetDocumentHandler : IRequestHandler<GetAllAssetEquipmentMaintenaceMasterAssetDocumentQuery, List<AssetEquipmentMaintenaceMasterAssetDocument>>
    {
        private readonly IAssetEquipmentMaintenaceMasterAssetDocumentQueryRepository _queryRepository;
        public GetAllAssetEquipmentMaintenaceMasterAssetDocumentHandler(IAssetEquipmentMaintenaceMasterAssetDocumentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<AssetEquipmentMaintenaceMasterAssetDocument>> Handle(GetAllAssetEquipmentMaintenaceMasterAssetDocumentQuery request, CancellationToken cancellationToken)
        {
            return (List<AssetEquipmentMaintenaceMasterAssetDocument>)await _queryRepository.GetAllByIdAsync(request.AssetEquipmentMaintenaceMasterId);
        }
    }
}
