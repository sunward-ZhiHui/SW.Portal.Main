using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class FoldersModel : BaseModel
    {
        public long FolderID { get; set; }
        public long? MainFolderID { get; set; }
        public long? ParentFolderID { get; set; }
        public int? FolderTypeID { get; set; }
        public int? CodeID { get; set; }
        public string CodeType { get; set; }
        public string Name { get; set; }
        public string ParentFolderName { get; set; }
        public string FolderTypeName { get; set; }
        public string Description { get; set; }
        public Guid? SessionID { get; set; }
        public List<FoldersModel> Children { get; set; } = new List<FoldersModel>();
        public bool HasPermission { get; set; }
        public long? Id { get; set; }
        public string ParentFolderPathName { get; set; }

        //Document / File Upload Requirements
        public List<long> AttachmentIds { get; set; }
        public List<long?> Attachments { get; set; }
        public List<long?> ReadonlyUserID { get; set; }
        public List<long?> ReadWriteUserID { get; set; }
        public long? DocumentFolderID { get; set; }

        public List<string> DocSessionID { get; set; }
        public List<string> FileNames { get; set; } = new List<string>();
        public string DocumentDescription { get; set; }

        //Set Access Rights
        public long? DocumentUserRoleId { get; set; }
        public long? DocumentId { get; set; }
        public long? UserId { get; set; }
        public long? DocumentRoleId { get; set; }
        public long? UserGroupId { get; set; }
        public List<long?> UserIDs { get; set; }
        public List<long?> UserGroupIDs { get; set; }
        public bool? IsFolderLevel { get; set; }
        public string FolderPath { get; set; }
        public List<string> FolderPaths { get; set; } = new List<string>();
        public List<string> FolderPathLists { get; set; } = new List<string>();
        public bool? IsRead { get; set; } = true;
        public bool? IsCreateDocument { get; set; } = true;
        public bool? IsSetAlert { get; set; } = true;
        public bool? IsRename { get; set; } = true;
        public bool? IsUpdateDocument { get; set; } = true;
        public bool? IsCopy { get; set; } = true;
        public bool? IsEdit { get; set; } = true;
        public bool? IsMove { get; set; } = true;
        public bool? IsDelete { get; set; } = true;
        public bool? IsRelationship { get; set; } = true;
        public bool? IsListVersion { get; set; } = true;
        public bool? IsInvitation { get; set; } = true;
        public bool? IsDiscussion { get; set; } = true;
        public bool? IsAccessControl { get; set; } = true;
        public bool? IsFileDelete { get; set; } = true;
        public bool? IsAuditTrail { get; set; } = true;
        public string cssTitle { get; set; }

        public int TreeLevel { get; set; }

 //       public List<DocumentDetailsModel> DocumentDetails { get; set; }

        public List<long?> FolderList { get; set; }

        public long? LevelID { get; set; }
    }
}
