using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormApproved
    {
        [Key]
        public long DynamicFormApprovedId { get; set; }
        public long? DynamicFormApprovalId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public bool? IsApproved { get; set; }
        
        public long? UserId { get; set; }
        public long? ApprovedByUserId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedDescription { get; set; }
        [NotMapped]
        public long? DynamicFormId { get; set; }
        [NotMapped]
        public long? ApprovalUserId { get; set; }
        [NotMapped]
        public string? EmployeeStatus { get; set; }
        [NotMapped]
        public string? ApprovedStatus { get; set; }
        [NotMapped]
        public string? ApprovalUser { get; set; }
        [NotMapped]
        public string? ApprovedByUser { get; set; }
        public int? ApprovedSortBy { get; set; }
        public long? DelegateApproveUserId { get; set; }
        public string? DelegateApproveUserName { get; set; }
        public bool? IsDelegateApproveStatus { get; set; }
        public long? DelegateApprovedChangedId { get; set; }
        public List<DynamicFormApprovedChanged> DynamicFormApprovedChangeds { get; set; } = new List<DynamicFormApprovedChanged>();
        public string? DelegateApproveAllUserName { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public long? DelegateApproveAllUserId { get; set; }
        public int? TotalApproval { get; set; }=0;
        public int? CompletedApproval { get; set; } = 0;
        public bool? IsPendingApproval { get; set; } = false;
        public string? ProfileNo { get; set; }
        public string? FormName { get; set; }
        public bool? IsDelegateUser { get; set; } = false;
        public Guid? FormSessionId { get; set; }
        public Guid? FormDataSessionId { get; set; }
    }

}
