using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentPermission
    {
        [Key]
        public long DocumentPermissionId { get; set; }
        public long? DocumentId { get; set; }
        public bool? IsRead { get; set; } = false;
        public bool? IsCreateFolder { get; set; } = false;
        public bool? IsCreateDocument { get; set; } = false;
        public bool? IsSetAlert { get; set; } = false;
        public bool? IsEditIndex { get; set; } = false;
        public bool? IsRename { get; set; } = false;
        public bool? IsUpdateDocument { get; set; } = false;
        public bool? IsCopy { get; set; } = false;
        public bool? IsMove { get; set; } = false;
        public bool? IsDelete { get; set; } = false;
        public bool? IsRelationship { get; set; } = false;
        public bool? IsListVersion { get; set; } = false;
        public bool? IsInvitation { get; set; } = false;
        public bool? IsSendEmail { get; set; } = false;
        public bool? IsDiscussion { get; set; } = false;
        public bool? IsAccessControl { get; set; } = false;
        public bool? IsAuditTrail { get; set; } = false;
        public bool? IsRequired { get; set; } = false;
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? FolderId { get; set; }
        public long? DocumentRoleId { get; set; }
        public bool? IsFileDelete { get; set; } = false;
        public bool? IsEdit { get; set; } = false;
        public bool? IsGrantAdminPermission { get; set; } = false;
        public bool? IsDocumentAccess { get; set; } = false;
        public bool? IsCreateTask { get; set; } = false;
        public bool? IsEnableProfileTypeInfo { get; set; } = false;
        public bool? IsShare { get; set; } = false;
        public bool? IsCloseDocument { get; set; } = false;
        public bool? IsEditFolder { get; set; } = false;
        public bool? IsDeleteFolder { get; set; } = false;
        public bool? IsReserveProfileNumber { get; set; } = false;
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public string? DocumentRoleName { get; set; }
    }
}
