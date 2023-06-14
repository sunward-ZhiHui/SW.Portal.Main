using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AssetEquipmentMaintenaceMasterAssetDocument
    {
        [Key]
        public long AssetEquipmentMaintenaceMasterAssetDocumentId { get; set; }
        public long? AssetEquipmentMaintenaceMasterId { get; set; }
        public long? AssetDocumentId { get; set; }
    }
}
