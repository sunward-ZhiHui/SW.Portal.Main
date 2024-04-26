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
      
        public long? MasterProductionFileProfileTypeId { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedByUser { get; set; }
        [DisplayName("Modified Date")]
      
        public DateTime? ModifiedDate { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public long ProductionActivityRoutineAppId { get; set; }
        public long? ManufacturingProcessChildId { get; set; }
        public long? ProdActivityCategoryChildId { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public bool IsTemplateUpload { get; set; } = false;
        public long? ProductActivityCaseLineId { get; set; }
        public string? LineSessionId { get; set; }
        public bool? IsOthersOptions { get; set; }
        public long? ProdActivityResultId { get; set; }
        public long? RoutineStatusId { get; set; }
        public string? LocationName { get; set; }
        public string? RoutineInfoStatus { get; set; }
        public string? IsTemplateUploadFlag { get; set; }
        public string? NameOfTemplate { get; set; }
        public string? OthersOptions { get; set; }
        public string? RoutineStatus { get; set; }
        public IEnumerable<long?> RoutineInfoIds { get; set; } = new List<long?>();
    }
}
