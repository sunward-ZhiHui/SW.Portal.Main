using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormWorkFlow
    {
        [Key]
        public long DynamicFormWorkFlowId { get; set; }
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? LevelId { get; set; }
        [Required(ErrorMessage = "Sequence No is required")]
        public int? SequenceNo { get; set; }
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
        public IEnumerable<long>? SelectUserIDs { get; set; } = new List<long>();
        [NotMapped]
        public IEnumerable<long>? SelectUserGroupIDs { get; set; } = new List<long>();
        [NotMapped]
        public IEnumerable<long>? SelectLevelMasterIDs { get; set; } = new List<long>();
        [NotMapped]
        [Required(ErrorMessage = "Section Name is required")]
        public IEnumerable<long>? SelectDynamicFormSectionIDs { get; set; } = new List<long>();
        public List<long> DynamicFormSectionIDs { get; set; } = new List<long>();
        public List<long> DynamicFormSectionIdList { get; set; } = new List<long>();
        public string? Type { get; set; }

    }
    public class DynamicFormWorkFlowSection
    {
        [Key]
        public long DynamicFormWorkFlowId { get; set; }
        public long? DynamicFormWorkFlowSectionId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        [NotMapped]
        public long? UserGroupId { get; set; }
        [NotMapped]
        public long? LevelId { get; set; }
        public string? Type { get; set; }
        public int? IsWorkFlowDone { get; set; }
        public int? SequenceNo { get; set; }
        public string? SectionName { get; set; }
        public string? UserName { get; set; }
        public long? DynamicFormDataId { get; set; }
    }
    public class DynamicFormWorkFlowForm
    {
        [Key]
        public long DynamicFormWorkFlowFormId { get; set; }
        public long? DynamicFormWorkFlowSectionId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? UserId { get; set; }
        public DateTime? CompletedDate { get; set; }
        [NotMapped]
        public string? CompletedBy { get; set; }
        [NotMapped]
        public int? SequenceNo { get; set; }
        [NotMapped]
        public long? DynamicFormSectionId { get; set; }
        [NotMapped]
        public long? DynamicFormWorkFlowId { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public long? DynamicFormWorkFlowUserId { get; set; }
        [NotMapped]
        public string? DynamicFormWorkFlowUser { get; set; }
        public int? RowID { get; set; }
    }
}
