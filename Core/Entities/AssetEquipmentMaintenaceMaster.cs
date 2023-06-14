using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AssetEquipmentMaintenaceMaster
    {
        [Key]
        public long AssetEquipmentMaintenaceMasterId { get; set; }
        public long? AssetPartsMaintenaceMasterId { get; set; }
        public string? IsCalibarion { get; set; }
        public long? PreventiveMaintenaceId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
