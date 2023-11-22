using Core.Entities;
using Core.Entities.CustomValidations;
using Core.Entities.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductActivityAppModel : BaseModel
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
        [Required(ErrorMessage = "Name Of Template is required")]
        public long? ProductActivityCaseLineId { get; set; }
        public string? NameOfTemplate { get; set; }
        public long? DocumentId { get; set; }
        public string? Comment { get; set; }
        [Required(ErrorMessage = "Prod OrderNo is required")]
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
        [Required(ErrorMessage = "Other Options is required")]
        public string? OthersOptions { get; set; }
        public bool? IsOthersOptions { get; set; } = null;
        public long? DocumentParentId { get; set; }
        public string? FileName { get; set; }
        public string? ProfileNo { get; set; }
        
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
        [Required(ErrorMessage = "Manufacturing Process is Required")]
        public long? ManufacturingProcessChildId { get; set; }
        [Required(ErrorMessage = "Category is Required")]
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
    }
    public class MultipleProductioAppLineItemLists
    {
        public List<ProductActivityCaseRespons> ProductActivityCaseRespons { get; set; } = new List<ProductActivityCaseRespons>();
        public List<ProductActivityCaseResponsDuty> ProductActivityCaseResponsDuty { get; set; } = new List<ProductActivityCaseResponsDuty>();
        public List<ProductActivityCaseResponsResponsible> ProductActivityCaseResponsResponsible { get; set; } = new List<ProductActivityCaseResponsResponsible>();
        public List<ActivityMasterMultiple> ActivityMasterMultiple { get; set; } = new List<ActivityMasterMultiple>();
        public List<ProductionActivityAppLineQaCheckerModel> ProductionActivityAppLineQaCheckerModel { get; set; } = new List<ProductionActivityAppLineQaCheckerModel>();
        public List<Documents> Documents { get; set; } = new List<Documents>();
        public List<ApplicationUser> ApplicationUser { get; set; } = new List<ApplicationUser>();
        public List<NavprodOrderLine> NavprodOrderLine { get; set; } = new List<NavprodOrderLine>();
        public List<CodeMaster> CodeMaster { get; set; } = new List<CodeMaster>();
        public List<ApplicationMasterChild> ApplicationMasterChild { get; set; } = new List<ApplicationMasterChild>();
        public List<ApplicationMasterDetail> ApplicationMasterDetail { get; set; } = new List<ApplicationMasterDetail>();
        public List<ActivityEmailTopicsModel> ActivityEmailTopics { get; set; } = new List<ActivityEmailTopicsModel>();
        public List<ProductActivityPermissionModel> ProductActivityPermission { get; set; } = new List<ProductActivityPermissionModel>();
        public List<ProductActivityCaseCategoryMultiple> ProductActivityCaseCategoryMultiple { get; set; } = new List<ProductActivityCaseCategoryMultiple>();
        public List<ProductActivityCaseActionMultiple> ProductActivityCaseActionMultiple { get; set; } = new List<ProductActivityCaseActionMultiple>();
        public List<ProductActivityCase> ProductActivityCase { get; set; } = new List<ProductActivityCase>();
    }
}
