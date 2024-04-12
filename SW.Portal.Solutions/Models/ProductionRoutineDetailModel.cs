using System.ComponentModel;

namespace SW.Portal.Solutions.Models
{
    public class ProductionRoutineDetailModel
    {
      public long CompanyId { get; set; }
        public string LotNo { get; set;}
        public string ItemName { get; set; }
        public long AddedByUserID { get; set; }
        public string GetTypes { get; set;}
        public long? LocationId { get; set; }
        public bool TimeSheetAction { get; set; }
        public string? Type { get; set; }
        public string? ActivityProfileNo { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? ManufacturingProcessChild { get; set; }
        public string? ProdActivityCategoryChild { get; set; }
        public string? ProdActivityActionChild { get; set; }
        public string? LineComment { get; set; }
        public string? ProdActivityResult { get; set; }

        [DisplayName("Modified By")]
        public string ModifiedByUser { get; set; }
        [DisplayName("Modified Date")]
      
        public DateTime? ModifiedDate { get; set; }
    }
}
