using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OpenAccessUser : BaseEntity
    {
        [Key]
        public long OpenAccessUserId { get; set; }
        public string? AccessType { get; set; }
    }
    public class OpenAccessUserLink:BaseEntity
    {
        [Key]
        public long OpenAccessUserLinkId { get; set; }
        public long? OpenAccessUserId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? LevelId { get; set; }
        public bool? IsDmsCreateMainFolder { get; set; }=false;
        public bool? IsDmsAccess { get; set; } = false;
        public string? UserName { get; set; }
        [NotMapped]
        public string? AccessType { get; set; }
        [NotMapped]
        public string? OpenAccessUser { get; set; }
        [NotMapped]
        public string? Type { get; set; }
        [NotMapped]
        public IEnumerable<long>? SelectUserIDs { get; set; }
        [NotMapped]
        public IEnumerable<long>? SelectUserGroupIDs { get; set; }
        [NotMapped]
        public IEnumerable<long>? SelectLevelMasterIDs { get; set; }
        [NotMapped]
        public string? UserGroup { get; set; }
        [NotMapped]
        public string? UserGroupDescription { get; set; }
        [NotMapped]
        public string? LevelName { get; set; }
        [NotMapped]
        public string? NickName { get; set; }
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string? DesignationName { get; set; }
        [NotMapped]
        public string? FullName { get; set; }
        public bool? IsAdd { get; set; } = false;
        public bool? IsEdit { get; set; } = false;
        public bool? IsDelete { get; set; } = false;
    }
}
