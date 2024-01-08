using Core.EntityModels;

namespace SW.Portal.Solutions.Models
{
    public class EmailTopicViewModel
    {
        public long id { get; set; }
        public long replyId { get; set; }
        public string? topicName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public bool? urgent { get; set; }
        public bool? isAllowParticipants { get; set; }
        public DateTime? dueDate { get; set; }
        public long? onBehalf { get; set; }
        public string? onBehalfName { get; set; }
        public DateTime? addedDate { get; set; }
        public long? addedByUserID { get; set; }
        public string? mode { get;set; }
        public long userId { get; set; }
        public int? addedDateYear { get; set; }        
        public string? addedDateDay { get; set; }
        public string? addedTime { get; set; }
    }
    public class ProductActivitysAppModel 
    {
        public long ProductionActivityAppId { get; set; }
        public long? CompanyId { get; set; }
        public long? ProdActivityActionId { get; set; }
        public string? ActionDropdown { get; set; }
        public long? ProdActivityCategoryId { get; set; }
        public long? ManufacturingProcessId { get; set; }
        public bool IsTemplateUpload { get; set; } = false;
        public string? ProdOrderNo { get; set; }
        public string? ProdActivityAction { get; set; }
        public string? ProdActivityCategory { get; set; }
        public string? ManufacturingProcess { get; set; }
        public string? IsTemplateUploadFlag { get; set; } = "No";
        public long? ProductActivityCaseLineId { get; set; }
        public string? NameOfTemplate { get; set; }
        public long? DocumentId { get; set; }
        public string? Comment { get; set; }
        public long? NavprodOrderLineId { get; set; }
        public Guid? LineSessionId { get; set; }
        public string? LineComment { get; set; }
        public long? ProductionActivityAppLineId { get; set; }
        public string? Link { get; set; }
        public bool? QaCheck { get; set; } = false;
        public string? RePlanRefNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public string? BatchNo { get; set; }
        public long? LocationToSaveId { get; set; }
        public string? OthersOptions { get; set; }
        public bool? IsOthersOptions { get; set; } = null;
        public long? DocumentParentId { get; set; }
        public string? FileName { get; set; }
        public string? ActivityProfileNo { get; set; }
        public string? ProfileNo { get; set; }
        public long? ProfileId { get; set; }
        public string? ContentType { get; set; }
        public bool? IsLocked { get; set; }
        public long? LockedByUserId { get; set; }
        public string? LockedByUser { get; set; }
        public long? NotifyCount { get; set; }
        public long? ProdActivityResultId { get; set; }
        public string? ProdActivityResult { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TopicId { get; set; }
        public long? ManufacturingProcessChildId { get; set; }
        public long? ProdActivityCategoryChildId { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public string? ManufacturingProcessChild { get; set; }
        public string? ProdActivityCategoryChild { get; set; }
        public string? ProdActivityActionChild { get; set; }
        public DocumentPermissionModel DocumentPermissionData { get; set; } = new DocumentPermissionModel();
        public ProductActivityPermissionModel ProductActivityPermissionData { get; set; } = new ProductActivityPermissionModel();
        public List<ProductActivityPermissionModel> ProductActivityPermissions { get; set; } = new List<ProductActivityPermissionModel>();
        public string? ProdOrderNoDesc { get; set; }
        public string? Type { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public string? QaCheckUser { get; set; }
        public string? FilePath { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public bool? IsNewPath { get; set; }
        public string? LocationName { get; set; }
        public List<ProductionActivityAppLineQaCheckerModel> ProductionActivityAppLineQaCheckerModels { get; set; } = new List<ProductionActivityAppLineQaCheckerModel>();
        public long? LocationId { get; set; }
        public int? SupportDocCount { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public List<long> ResponsibilityUsers { get; set; } = new List<long>();
        public long? RoutineStatusId { get; set; }

        public List<long?> RoutineInfoIds { get; set; } = new List<long?>();
        public IEnumerable<long?> ActivityMasterIds { get; set; } = new List<long?>();
        public string? ActivityMaster { get; set; }
        public string? ActivityResult { get; set; }
        public string? ActivityStatus { get; set; }
        public long? ActivityMasterId { get; set; }
        public long? ActivityStatusId { get; set; }
        public byte[]? CommentImage { get; set; }
        public string? CommentImageType { get; set; }
        public bool? IsEmailCreated { get; set; } = false;
        public bool? IsActionPermission { get; set; } = true;
        public string? NoAction { get; set; }
        public long? FileProfileTypeId { get; set; }
        public int? IsDocuments { get; set; }
        public int? StatusCodeID { get; set; }
        public string StatusCode { get; set; }
        public long? AddedByUserID { get; set; }
        public string AddedByUser { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CompanyName { get; set; }
        //public long? DocumentID { get; set; }

        public Guid? SessionId { get; set; }
    }
}
