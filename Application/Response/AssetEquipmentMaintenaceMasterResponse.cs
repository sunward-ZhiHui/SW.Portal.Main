using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AssetEquipmentMaintenaceMasterResponse
    {
        public long AssetEquipmentMaintenaceMasterId { get; set; }
        public long? AssetPartsMaintenaceMasterId { get; set; }
        public int Index { get; set; }
        public string? IsCalibarion { get; set; }
        public long? PreventiveMaintenaceId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string PreventiveMaintenace { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int CodeID { get; set; }
    }
}
