using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormSectionAttributeSecurity
    {
        public long DynamicFormSectionAttributeSecurityId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? LevelId { get; set; }
        public bool IsAccess { get; set; } = false;
        public bool IsViewFormatOnly { get; set; } = false;
        public string? UserType { get; set; }
        [NotMapped]
        public string? UserGroup { get; set; }
        [NotMapped]
        public string? UserGroupDescription { get; set; }
        [NotMapped]
        public string? DisplayName { get; set; }
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
        [NotMapped]
        public IEnumerable<long?> SelectUserIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectUserGroupIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectLevelMasterIDs { get; set; } = new List<long?>();
    }
}
