using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AssetPartsMaintenaceMaster
    {
        [Key]
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
    }
}
