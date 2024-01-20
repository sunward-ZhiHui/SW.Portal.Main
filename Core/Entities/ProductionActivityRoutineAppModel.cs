using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionActivityRoutineAppModel : BaseModel
    {
        public long ProductionActivityRoutineAppId { get; set; }
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
        public string? IsTemplateUploadFlag { get; set; }
        public long? ProductActivityCaseLineId { get; set; }
        public string? NameOfTemplate { get; set; }
        public long? DocumentId { get; set; }
        public string? Comment { get; set; }
        public long? NavprodOrderLineId { get; set; }
        public Guid? LineSessionId { get; set; }
        public string? LineComment { get; set; }
        public string? OthersOptions { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public ProductActivityPermissionModel? ProductActivityPermissionData { get; set; }
        public string? Link { get; set; }
        public bool? QaCheck { get; set; } = false;
        public string? RePlanRefNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public string? BatchNo { get; set; }
        public long? LocationToSaveId { get; set; }
        public bool? IsOthersOptions { get; set; }
        public long? DocumentParentId { get; set; }
        public string? FileName { get; set; }
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
        public DocumentPermissionModel? DocumentPermissionData { get; set; }
        public string? ProdOrderNoDesc { get; set; }
        public string? Type { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public string? QaCheckUser { get; set; }
        public string? FilePath { get; set; }
        public List<ProductionActivityRoutineAppLineQaCheckerModel>? ProductionActivityRoutineAppLineQaCheckerModels { get; set; }
        public long? LocationId { get; set; }
        public string? LocationName { get; set; }
        public int? SupportDocCount { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public List<long> ResponsibilityUsers { get; set; } = new List<long>();
        public string? TicketNo { get; set; }
        public long? VisaMasterId { get; set; }
        public string? VisaMaster { get; set; }
        public long? RoutineStatusId { get; set; }
        public string? RoutineStatus { get; set; }
        public string? RoutineInfoStatus { get; set; }
        public IEnumerable<long?> RoutineInfoIds { get; set; } = new List<long?>();
        public byte[]? CommentImage { get; set; }
        public string? CommentImageType { get; set; }
        public bool? IsEmailCreated { get; set; } = false;
        public bool? IsPartialEmailCreated { get; set; } = false;

        public bool? IsActionPermission { get; set; } = true;
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? ActivityProfileNo { get; set; }
        public long? FileProfileTypeId { get; set; }
        public int? IsDocuments { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public bool? IsNewPath { get; set; }
        public long? CompanyID { get; set; }
        public long? LocationID { get; set; }
        public List<ProductActivityPermissionModel>? ProductActivityPermissions { get; set; }
        public bool? IsCheckNoIssue { get; set; } = false;
        public bool? IsCheckReferSupportDocument { get; set; } = false;
        public long? CheckedById { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string? CheckedRemark { get; set; }
        public string? CheckedByUser { get; set; }
        public string? GetTypes { get; set; }
        public Guid? EmailSessionId { get; set; }
        public Guid? EmailActivitySessionId { get; set; }
    }
    public class ProductionActivityRoutineAppLineQaCheckerModel
    {
        public long ProductionActivityRoutineAppLineQaCheckerId { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public bool? QaCheck { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public string QaCheckUser { get; set; }
        public string QaCheckFlag { get; set; }
    }
    public class ProductionActivityRoutineAppsModel : BaseModel
    {
        public long ProductionActivityRoutineAppId { get; set; }
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
        public string? IsTemplateUploadFlag { get; set; }
        public long? ProductActivityCaseLineId { get; set; }
        public string? NameOfTemplate { get; set; }
        public string? Comment { get; set; }
        public long? NavprodOrderLineId { get; set; }
        public Guid? LineSessionId { get; set; }
        public string? LineComment { get; set; }
        public string? OthersOptions { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public ProductActivityPermissionModel? ProductActivityPermissionData { get; set; }
        public string? Link { get; set; }
        public bool? QaCheck { get; set; } = false;
        public string? RePlanRefNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public string? BatchNo { get; set; }
        public long? LocationToSaveId { get; set; }
        public bool? IsOthersOptions { get; set; }
        public long? DocumentParentId { get; set; }
        public string? FileName { get; set; }
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
        public DocumentPermissionModel? DocumentPermissionData { get; set; }
        public string? ProdOrderNoDesc { get; set; }
        public string? Type { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public string? QaCheckUser { get; set; }
        public string? FilePath { get; set; }
        public List<ProductionActivityRoutineAppLineQaCheckerModel>? ProductionActivityRoutineAppLineQaCheckerModels { get; set; }
        public long? LocationId { get; set; }
        public string? LocationName { get; set; }
        public int? SupportDocCount { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public List<long> ResponsibilityUsers { get; set; } = new List<long>();
        public string? TicketNo { get; set; }
        public long? VisaMasterId { get; set; }
        public string? VisaMaster { get; set; }
        public long? RoutineStatusId { get; set; }
        public string? RoutineStatus { get; set; }
        public string? RoutineInfoStatus { get; set; }
        public IEnumerable<long?> RoutineInfoIds { get; set; } = new List<long?>();
        public byte[]? CommentImage { get; set; }
        public string? CommentImageType { get; set; }
        public bool? IsEmailCreated { get; set; } = false;

        public bool? IsActionPermission { get; set; } = true;
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? ActivityProfileNo { get; set; }
        public long? FileProfileTypeId { get; set; }
        public int? IsDocuments { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public bool? IsNewPath { get; set; }
        public long? CompanyID { get; set; }
        public long? LocationID { get; set; }
        public List<ProductActivityPermissionModel>? ProductActivityPermissions { get; set; }
        public bool? IsCheckNoIssue { get; set; } = false;
        public bool? IsCheckReferSupportDocument { get; set; } = false;
        public long? CheckedById { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string? CheckedRemark { get; set; }
        public string? CheckedByUser { get; set; }
        public string? GetTypes { get; set; }
        public Guid? EmailSessionId { get; set; }
        public bool? IsPartialEmailCreated { get; set; } = false;
        public Guid? EmailActivitySessionId { get; set; }
    }
    public class ProductionActivityRoutineAppStatusModel
    {
        public long ProductionActivityRoutineAppLineId { get; set; }
        public long? RoutineStatusId { get; set; }
    }
}
