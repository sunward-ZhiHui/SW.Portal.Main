using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.AssetPartsMaintenaceMasters
{
    public class EditAssetPartsMaintenaceMasterCommand : IRequest<AssetPartsMaintenaceMasterResponse>
    {
        public long AssetPartsMaintenaceMasterId { get; set; }
        public long? AssetCatalogMasterId { get; set; }
        public long? AssetCatalogId { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public EditAssetPartsMaintenaceMasterCommand()
        {

        }
    }
    public class EditAssetEquipmentMaintenaceMasterCommand : IRequest<AssetEquipmentMaintenaceMasterResponse>
    {
        public long AssetEquipmentMaintenaceMasterId { get; set; }
        public long? AssetPartsMaintenaceMasterId { get; set; }
        public string? IsCalibarion { get; set; }
        public long? PreventiveMaintenaceId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IEnumerable<long?> AssetDocumentationIds { get; set; }
        public EditAssetEquipmentMaintenaceMasterCommand()
        {

        }
    }
}
