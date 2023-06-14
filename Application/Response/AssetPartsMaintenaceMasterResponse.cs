using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AssetPartsMaintenaceMasterResponse
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
    }
}
