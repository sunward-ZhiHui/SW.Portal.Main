using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AssetCatalogMasterResponse
    {
        public long AssetCatalogMasterId { get; set; }
        public long? CompanyId { get; set; }
        public long? AssetCatalogId { get; set; }
        public long? AssetSectionId { get; set; }
        public long? AssetSubSectionId { get; set; }
        public string AssetDescription { get; set; }
        public long? AssetModelId { get; set; }
        public string ModelNo { get; set; }
        public long? AssetUomid { get; set; }
        public string FinanaceFixAssetNo { get; set; }
        public bool? NoFinanceFixAssetNo { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AssetGroupingId { get; set; }
        public long? AssetTypeId { get; set; }
        public long? AssetCategoryId { get; set; }
        public string? AssetCatalogNo { get; set; }
    }
}
