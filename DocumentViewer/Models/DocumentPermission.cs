using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class DocumentPermission
    {
        [Key]
        public long DocumentPermissionId { get; set; }
        public long? DocumentId { get; set; }
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
        public bool? IsRequired { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? FolderId { get; set; }
        public long? DocumentRoleId { get; set; }
        public bool? IsFileDelete { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsGrantAdminPermission { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public bool? IsCreateTask { get; set; }
        public bool? IsEnableProfileTypeInfo { get; set; }
        public bool? IsShare { get; set; }
        public bool? IsCloseDocument { get; set; }
    }
}
