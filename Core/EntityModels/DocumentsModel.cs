using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentsModel : BaseModel
    {
        public long UniqueNo { get; set; }
        public long DocumentID { get; set; }
        public long? DepartmentID { get; set; }
        public long? ParentId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? WikiID { get; set; }
        public long? CategoryID { get; set; }
        public string? FileName { get; set; }
        public string? BreadcumName { get; set; }
        public string? OriginalFileName { get; set; }
        public string DocumentName { get; set; }
        public string DisplayName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public int? DocumentType { get; set; }
        public byte?[] FileData { get; set; }
        public byte[] ImageData { get; set; }
        public int? FileIndex { get; set; }
        public long? FileSize { get; set; }
        public DateTime? UploadDate { get; set; }
        public Guid? SessionID { get; set; }
        public long? LinkID { get; set; }
        public long? AppWikiReleaseDocId { get; set; }
        public long? ParentTaskID { get; set; }
        public string UserAccess { get; set; }
        public bool? IsSpecialFile { get; set; }
        public bool IsSelected { get; set; }
        public List<long?> UserID { get; set; }
        public bool HasAccess { get; set; }
        public long? CompanyCalendarLineId { get; set; }
        public bool? IsTemp { get; set; }
        public bool? Checked { get; set; } = false;
        public int? ReferenceNumber { get; set; }
        public string Description { get; set; }
        public List<DocumentsModel> Children { get; set; }
        public List<long> AttachmentIds { get; set; }
        public bool IsImage { get; set; }

        public long? TaskAttachmentId { get; set; }
        public long? SelectedFolderID { get; set; }

        public List<long> DocumentFolderIds { get; set; }
        public long? DocumentFolderId { get; set; }
        public string LinkedFolder { get; set; }
        public string FullFolderPath { get; set; }
        public string FolderName { get; set; }
        public string LinkedTask { get; set; }
        public string SubjectName { get; set; }
        public string VersionNo { get; set; }
        public string ActualVersionNo { get; set; }
        public string DraftingVersionNo { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsMajorChange { get; set; }
        public bool? IsNoChange { get; set; }
        public bool? IsReleaseVersion { get; set; }

        public long? LockedByUserId { get; set; }
        public DateTime? LockedDate { get; set; }

        public bool? IsMeetingNotes { get; set; }
        public bool? IsDiscussionNotes { get; set; }
        public bool? IsSubTask { get; set; }

        public long? PreviousDocumentId { get; set; }

        public string UploadedByUser { get; set; }

        public string LockedByUser { get; set; }

        public long? UploadedByUserId { get; set; }
        public bool? IsPrint { get; set; }
        public List<long> ReadOnlyUserId { get; set; } = new List<long>();
        public List<long> NoAccessId { get; set; } = new List<long>();
        public List<long> ReadWriteUserId { get; set; } = new List<long>();
        public DocumentPermissionModel DocumentPermission { get; set; }
        public string DocumentPath { get; set; }
        public bool? IsWikiDraft { get; set; }
        public int? ArchiveStatusId { get; set; }
        //DocumentPermission Fields
        public bool? IsWiki { get; set; }
        public bool? IsWikiDraftDelete { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsCreateFolder { get; set; }
        public bool? IsCreateDocument { get; set; }
        public bool? IsSetAlert { get; set; }
        public bool? IsEditIndex { get; set; }
        public bool? IsRename { get; set; }
        public bool? IsUpdateDocument { get; set; }
        public bool? IsCopy { get; set; }
        public bool? IsMove { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsRelationship { get; set; }
        public bool? IsListVersion { get; set; }
        public bool? IsInvitation { get; set; }
        public bool? IsSendEmail { get; set; }
        public bool? IsDiscussion { get; set; }
        public bool? IsAccessControl { get; set; }
        public bool? IsAuditTrail { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsFileDelete { get; set; }

        public long? FolderCreatedBy { get; set; }
        //DocumentUserRole Details
        public long DocumentUserRoleID { get; set; }
        public long? RoleID { get; set; }
        public long? UserGroupID { get; set; }
        public long? FolderID { get; set; }
        public long? ParentFolderID { get; set; }

        public bool IsSubTaskDocument { get; set; }

        public bool? IsNoAccess { get; set; }
        public bool? isSetAccess { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        // Add New User AssignTask Permission
        public List<long?> AssignedToIds { get; set; }
        public List<long?> AssignedCCIds { get; set; }

        public List<DocumentsModel> DocumentList { get; set; }
        public long? OnBehalfID { get; set; }

        public List<string> FileNames { get; set; }

        public string CheckInDescription { get; set; }

        public bool? Uploaded { get; set; }

        public long? FilterProfileTypeId { get; set; }
        public string FileProfileTypeName { get; set; }
        public string ProfileNo { get; set; }
        public long? ProfileID { get; set; }
        public long? DynamicFormId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public long? DocumentParentId { get; set; }
        public DocumentPermissionModel DocumentPermissionData { get; set; }
        public bool? IsLatest { get; set; }
        public long? TotalDocument { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? CanDownload { get; set; }
        public bool? IsExpiryDate { get; set; }
        public string WikiTitle { get; set; }
        public string WikiType { get; set; }
        public string WikiOwner { get; set; }
        public bool? IsMobileUpload { get; set; }
        public bool? IsCompressed { get; set; }
        public bool? IsVideoFile { get; set; }
        public bool? isDocumentAccess { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public int? CloseDocumentId { get; set; }
        public string CloseStatus { get; set; }
        public bool? IsMoveTo { get; set; }
        public string CssClass { get; set; }
        public bool? IsPublichFolder { get; set; }
        public long? FolderId { get; set; }
        public long? MainFolderID { get; set; }
        public long? TaskId { get; set; }
        public bool? IsMainTask { get; set; }
        public string DocumentLinkType { get; set; }
        public long? FileProfileTypeParentId { get; set; }
        public string TypeOfEventName { get; set; }
        public string CalenderStatusName { get; set; }
        public string TypeOfServiceName { get; set; }
        public long? CompanyCalendarLineLinkUserLinkId { get; set; }
        public bool? SetAccessFlag { get; set; }
        public long? FileProfileTypeAddedByUserId { get; set; }
        public bool? ItemsFlag { get; set; } = false;
        public bool? ItemsAllFlag { get; set; } = false;
        public bool? ItemsWithCreateTask { get; set; } = false;
        public bool? ItemsAllWithCreateTask { get; set; } = false;
        public bool? CalandarPermissionFlag { get; set; } = false;
        public long? NotesCount { get; set; }
        public long? SharesCount { get; set; }
        public string NotesColor { get; set; }
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
    }
    public class DocumentListModel : BaseModel
    {
        public List<DocumentsModel> DocumentList { get; set; }
        public long? FilterProfileTypeId { get; set; }
        public string Description { get; set; }
    }
    public class DocumentNameModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Extension { get; set; }
    }
    public class FormDropDownModel
    {
        public DocumentsModel FormEntity { get; set; }
    }
    public class DocumentTypeModel : BaseModel
    {
        public List<DocumentsModel> DocumentsData { get; set; } = new List<DocumentsModel>();
        public DocumentPermissionModel DocumentPermissionData { get; set; }
        public int? TotalDocument { get; set; }
        public int? OpenDocument { get; set; }

        public bool? IsExpiryDate { get; set; }
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

    }
    public class MultipleFileProfileItemLists
    {
        public List<DocumentDmsShare> DocumentDmsShare { get; set; } = new List<DocumentDmsShare>();
        public List<DynamicFormData> DynamicFormData { get; set; } = new List<DynamicFormData>();
        public List<ActivityEmailTopics> ActivityEmailTopics { get; set; } = new List<ActivityEmailTopics>();
        public List<ApplicationUser> ApplicationUser { get; set; } = new List<ApplicationUser>();
        public List<Fileprofiletype> Fileprofiletype { get; set; } = new List<Fileprofiletype>();
        public List<ProductActivityAppModel> ProductActivityAppModel { get; set; } = new List<ProductActivityAppModel>();
    }
}
