using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentsModel 
    {
        public long? DeleteByUserID { get; set; }
        public string? DeleteByUser { get; set; }
        public DateTime? DeleteByDate { get; set; }
        public long? AddedByUserID { get; set; }        
        public string AddedByUser { get; set; }       
        public DateTime? AddedDate { get; set; }       
        public string ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }       
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeID { get; set; }
        public long? DocumentID { get; set; }
        public string Name { get; set; }
        public long UniqueNo { get; set; }
        public long? ParentId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
        public int? FileIndex { get; set; }
        public long? FileSize { get; set; }
        public long? OriginalFileSize { get; set; }
        public DateTime? UploadDate { get; set; }
        public Guid? SessionID { get; set; }
        public bool Checked { get; set; } = false;
        public string Description { get; set; }
        public List<DocumentsModel> Children { get; set; }
        public bool? IsLocked { get; set; }
        public long? LockedByUserId { get; set; }
        public DateTime? LockedDate { get; set; }
        public string LockedByUser { get; set; }
        public long? UploadedByUserId { get; set; }
        public string DocumentPath { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public long? FilterProfileTypeId { get; set; }
        public string FileProfileTypeName { get; set; }
        public string ProfileNo { get; set; }
        public long? ProfileID { get; set; }
        public long? DynamicFormId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public long? DocumentParentId { get; set; }
        public long? TotalDocument { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsExpiryDate { get; set; }
        public bool? IsMobileUpload { get; set; }
        public bool? IsCompressed { get; set; }
        public bool? isDocumentAccess { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public int? CloseDocumentId { get; set; }
        public string CssClass { get; set; }
        public long? FolderId { get; set; }
        public long? FileProfileTypeParentId { get; set; }
        public long? FileProfileTypeAddedByUserId { get; set; }
        public long? SharesCount { get; set; }
        public string FilePath { get; set; }
        public string? FileSizes { get; set; }
        public string? FileCounts { get; set; }
        public bool? IsNewPath { get; set; }
        public long? FileProfileTypeMainParentId { get; set; }
        public string? FullPath { get; set; }
        public Guid? FileProfileTypeSessionId { get; set; }
        public string? SourceFrom { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public bool IsDynamicFromData { get; set; } = false;
        public bool IsEmailTopics { get; set; } = false;
        public bool HasChildren { get; set; } = false;
        public string? ActivityProfileNo { get; set; }
        public string? ActivityType { get; set; }
        public bool? IsDuplicateUpload { get; set; } = false;
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormSessionId { get; set; }
    }
    public class FormDropDownModel
    {
        public DocumentsModel FormEntity { get; set; }
    }
    public class DocumentTypeModel : BaseModel
    {
        public List<DocumentsModel> DocumentsData { get; set; } = new List<DocumentsModel>();
        public DocumentPermissionModel DocumentPermissionData { get; set; }
    }
    public class DocumentSearchModel
    {
        public string? FileName { get; set; }
        public long? FileProfileTypeId { get; set; }
        public Guid? SessionId { get; set; }
        public string? Extension { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ContentType { get; set; }
        public string? ProfileNo { get; set; }
        public List<long?> FileProfileTypeIds { get; set; }
        public List<Guid?> SessionIds { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? AttachSessionId { get; set; }

    }
    public class MultipleFileProfileItemLists
    {
        public List<DocumentDmsShare> DocumentDmsShare { get; set; } = new List<DocumentDmsShare>();
        public List<DynamicFormData> DynamicFormData { get; set; } = new List<DynamicFormData>();
        public List<ActivityEmailTopics> ActivityEmailTopics { get; set; } = new List<ActivityEmailTopics>();
        public List<ApplicationUser> ApplicationUser { get; set; } = new List<ApplicationUser>();
        public List<Fileprofiletype> Fileprofiletype { get; set; } = new List<Fileprofiletype>();
        public List<ProductActivityAppModel> ProductActivityAppModel { get; set; } = new List<ProductActivityAppModel>();
        public List<DynamicFormDataUpload> DynamicFormDataUpload { get; set; } = new List<DynamicFormDataUpload>();
    }
}
