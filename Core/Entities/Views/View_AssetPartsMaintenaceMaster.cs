using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_AssetPartsMaintenaceMaster
    {
        public long AssetPartsMaintenaceMasterId { get; set; }
        public long? AssetCatalogMasterId { get; set; }
        public int Index { get; set; }
        public long? AssetCatalogId { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string AssetCatalog { get; set; }
        public string AssetSection { get; set; }
        public string AssetSubSection { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int CodeID { get; set; }
    }
}
