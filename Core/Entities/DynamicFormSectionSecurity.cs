using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormSectionSecurity
    {
        [Key]
        public long DynamicFormSectionSecurityId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? LevelId { get; set; }
        [NotMapped]
        public bool IsReadWrite { get; set; } = false;
        [NotMapped]
        public bool IsReadOnly { get; set; } = false;
        [NotMapped]
        public bool IsVisible { get; set; } = false;
        [NotMapped]
        public bool IsRelease { get; set; } = false;

        [NotMapped]
        public string? UserGroup { get; set; }
        [NotMapped]
        public string? UserGroupDescription { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
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
        [NotMapped]
        public string? Type { get; set; }
        public string? FormName { get; set; }
        public long? DynamicFormId { get; set; }
        public long? AuditUserId { get; set; }
    }
    public class DynamicFormDataSectionSecurityRelease
    {
        [Key]
        public long DynamicFormDataSectionSecurityReleaseId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public long? UserId { get; set; }
        public bool IsRelease { get; set; } = false;
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        public string? ReleaseUser { get; set; }
        public string? SectionName { get; set; }
    }
}
