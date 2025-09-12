using Core.Entities.Base;
using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormApproval : BaseEntity
    {
        [Key]
        public long DynamicFormApprovalId { get; set; }
        public long? DynamicFormId { get; set; }
        [Required(ErrorMessage = "User is Required")]
        [DynamicFormApprovalCustomValidation]
        public long? ApprovalUserId { get; set; }
        public int? SortOrderBy { get; set; }
        public bool? IsApproved { get; set; } = false;
        public string? Description { get; set; }
        [NotMapped]
        public string? ApprovalUser { get; set; }
        [NotMapped]
        public int? SortOrderAnotherBy { get; set; }
        public int? ApprovedCountUsed { get; set; } = 0;
        [NotMapped]
        public int? Approved { get; set; }
        public string? FormName { get; set; }
        public long? AuditUserId { get; set; }
        public long? PreUserId { get; set; }
        public string? PreUserName { get; set; }
        public string? PreDescription { get; set; }

        public DateTime? PreModifiedDate { get; set; }
        public long? PreModifiedByUserID { get; set; }
        public string? PreModifiedBy { get; set; }
    }
}
