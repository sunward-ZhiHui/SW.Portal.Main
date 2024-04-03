using Core.Entities;
using Core.Entities.Base;
using Core.EntityModels;

namespace SW.Portal.Solutions.Models
{
    public class ReplyConversation
    {
        public long id { get; set; }
        public string Message { get; set; }

    }
    public class ProductionRoutine
    {
        public long ProductionActivityRoutineAppLineId { get; set; }
        public long? CompanyID { get; set; }
        public string? ProdOrderNo { get; set; }
        public long? LocationID { get; set; }
       
        public bool IsTemplateUpload { get; set; } = false;
       
        public bool IsTemplateUploadFlag { get; set; }
        public long? ProductActivityCaseLineId { get; set; }
        public long? NavprodOrderLineId { get; set; }
        public Guid? LineSessionId { get; set; }
        public string? LineComment { get; set; }
        public string? OthersOptions { get; set; }
        public long? ProductionActivityRoutineAppId { get; set; }
        public long? ProdActivityResultId { get; set; }
        public long? ManufacturingProcessChildId { get; set; }
        public long? ProdActivityCategoryChildId { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public string? ManufacturingProcessChild { get; set; }
        public string? ProdActivityCategoryChild { get; set; }
        public string? ProdActivityActionChild { get; set; }
      
       
        public long? ProductActivityCaseId { get; set; }
      
        public long? RoutineStatusId { get; set; }
        
        public IEnumerable<long?> RoutineInfoIds { get; set; } = new List<long?>();
     
        public bool? TimeSheetAction { get; set; } = false;
        public string? LotNo { get; set; }
        public string? ItemName { get; set; }
       public long? AddedByUserID { get; set; }

    }
}
