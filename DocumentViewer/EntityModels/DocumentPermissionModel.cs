namespace DocumentViewer.EntityModels
{
    public class DocumentPermissionModel 
    {
        public long DocumentPermissionID { get; set; }
        public long? DocumentID { get; set; }
        public string? DocumentName { get; set; }
        public long? DocumentRoleID { get; set; }
        public string? DocumentRoleDescription { get; set; }
        public string? DocumentRoleName { get; set; }
        public bool? IsRead { get; set; } = true;
        public bool? IsCreateFolder { get; set; } = true;
        public bool? IsCreateDocument { get; set; } = true;
        public bool? IsSetAlert { get; set; } = true;
        public bool? IsEditIndex { get; set; } = true;
        public bool? IsRename { get; set; } = true;
        public bool? IsUpdateDocument { get; set; } = true;
        public bool? IsCopy { get; set; } = true;
        public bool? IsMove { get; set; } = true;
        public bool? IsDelete { get; set; } = true;
        public bool? IsRelationship { get; set; } = true;
        public bool? IsListVersion { get; set; } = true;
        public bool? IsInvitation { get; set; } = true;
        public bool? IsSendEmail { get; set; } = true;
        public bool? IsDiscussion { get; set; } = true;
        public bool? IsAccessControl { get; set; } = true;
        public bool? IsAuditTrail { get; set; } = true;
        public bool? IsRequired { get; set; } = true;
        public bool? IsEdit { get; set; } = true;
        public bool? IsFileDelete { get; set; } = true;
        public bool? IsGrantAdminPermission { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public bool? IsCreateTask { get; set; }
        public long? FolderCreatedBy { get; set; }
        public long? FilelockedBy { get; set; }
        public bool? IsEnableProfileTypeInfo { get; set; }
        public bool? IsShare { get; set; }
        public bool? IsCloseDocument { get; set; }


        //DocumentUserRole
        public long DocumentUserRoleID { get; set; }
        public long? UserID { get; set; }
        public long? RoleID { get; set; }
        public long? UserGroupID { get; set; }
        public List<long?> UserGroupIDs { get; set; }
        public List<long?> UserIDs { get; set; }
        public long? FolderID { get; set; }
        public string? FolderName { get; set; }
        public long? FolderAddedByUserID { get; set; }
    }
}
