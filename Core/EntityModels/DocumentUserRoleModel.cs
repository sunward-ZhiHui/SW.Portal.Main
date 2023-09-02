using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentUserRoleModel
    {
        public long DocumentUserRoleID { get; set; }
        public long? UserID { get; set; }
        [Required(ErrorMessage = "Role is Required")]
        public long? RoleID { get; set; }
        public long? PreviousRoleID { get; set; }
        public long? FolderID { get; set; }
        public long? DocumentID { get; set; }
        public long? UserGroupID { get; set; }
        public bool? IsFolderLevel { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? FolderAddedByUserID { get; set; }
        public List<long?> UserIDs { get; set; }
        public List<long?> DocumetUserRoleIDs { get; set; }
        public List<long?> UserGroupIDs { get; set; }
        public bool IsSelected { get; set; } = false;
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string DepartmentName { get; set; }
        public string Designation { get; set; }
        public string UserGroupName { get; set; }
        public string FileProfileTypeName { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public List<long?> FileProfileTypeIds { get; set; }

        public long? LevelID { get; set; }
        public List<long?> DocumentIds { get; set; }
        public IEnumerable<long> SelectUserIDs { get; set; }
        public IEnumerable<long> SelectUserGroupIDs { get; set; }
        [Required(ErrorMessage = "Role is Required")]
        public long? UserGroupRoleID { get; set; }
        public IEnumerable<long> SelectLevelMasterIDs { get; set; }
        public long? LevelRoleID { get; set; }
        public string? Type { get; set; }

        public string? DocumentRoleName { get; set; }
        public string? DocumentRoleDescription { get; set; }
        public string? UserGroup { get; set; }
        public string? UserGroupDescription { get; set; }
        public string? FileProfileType { get; set; }
        public string? LevelName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DesignationName { get; set; }
        public string? FullName { get; set; }
    }
    public class LeveMasterUsersModel
    {
        public long? LevelId { get; set; }
        public long? DesignationId { get; set; }
        public long? UserId { get; set; }
    }
}
