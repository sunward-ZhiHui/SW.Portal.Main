using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_AssetCatalogMaster
    {
        public long AssetCatalogMasterId { get; set; }
        public int Index { get; set; }
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
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PlantCode { get; set; }
        public string CompanyName { get; set; }
        public int CodeID { get; set; }
        public string AssetCatalog { get; set; }
        public string AssetSection { get; set; }
        public string AssetSubSection { get; set; }
        public string AssetModel { get; set; }
        public string AssetUom { get; set; }
        public string AssetCategory { get; set; }
        public string AssetType { get; set; }
        public string AssetGrouping { get; set; }
        public string? AssetCatalogNo { get; set; }
    }
}
