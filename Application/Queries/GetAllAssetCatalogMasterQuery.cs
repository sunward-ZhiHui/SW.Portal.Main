using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAssetCatalogMasterQuery : PagedRequest, IRequest<List<View_AssetCatalogMaster>>
    {
        public string SearchString { get; set; }
        public long? CategoryId { get; set; }
        public long? SubSectionId { get; set; }
        public GetAllAssetCatalogMasterQuery(long? CategoryId, long? SubSectionId)
        {
            this.CategoryId = CategoryId;
            this.SubSectionId = SubSectionId;
        }
    }
    public class GetAllAssetPartsMaintenaceMasterQuery : PagedRequest, IRequest<List<View_AssetPartsMaintenaceMaster>>
    {
        public string SearchString { get; set; }
        public long? AssetCatalogMasterId { get; set; }
        public GetAllAssetPartsMaintenaceMasterQuery(long? AssetCatalogMasterId)
        {
            this.AssetCatalogMasterId = AssetCatalogMasterId;
        }
    }

    public class GetAllAssetEquipmentMaintenaceMasterQuery : PagedRequest, IRequest<List<View_AssetEquipmentMaintenaceMaster>>
    {
        public string SearchString { get; set; }
        public long? AssetPartsMaintenaceMasterId { get; set; }
        public GetAllAssetEquipmentMaintenaceMasterQuery(long? AssetPartsMaintenaceMasterId)
        {
            this.AssetPartsMaintenaceMasterId = AssetPartsMaintenaceMasterId;
        }
    }
    public class GetAllAssetEquipmentMaintenaceMasterAssetDocumentQuery : PagedRequest, IRequest<List<AssetEquipmentMaintenaceMasterAssetDocument>>
    {
        public string SearchString { get; set; }
        public long? AssetEquipmentMaintenaceMasterId { get; set; }
        public GetAllAssetEquipmentMaintenaceMasterAssetDocumentQuery(long? AssetEquipmentMaintenaceMasterId)
        {
            this.AssetEquipmentMaintenaceMasterId = AssetEquipmentMaintenaceMasterId;
        }
    }
    
}
