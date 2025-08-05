using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormWorkFlowApproval
    {
        public long DynamicFormWorkFlowApprovalId { get; set; }
        public long? DynamicFormWorkFlowId { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public long? UserId { get; set; }
        public int? SortBy { get; set; }
        public string? UserName { get; set; }
        public int? SortOrderAnotherBy { get; set; }
        public long DynamicFormWorkFlowApprovedFormID { get; set; }
        public long? DynamicFormWorkFlowFormID { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedDescription { get; set; }
        public long? ApprovedByUserID { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedStatus { get; set; }
        public string? ApprovedUser { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DelegateApproveUserId { get; set; }
        public string? DelegateApproveUserName { get; set; }
        public bool? IsDelegateApproveStatus { get; set; }
        public long? DelegateApprovedChangedId { get; set; }
        public List<DynamicFormWorkFlowApprovedFormChanged> DynamicFormWorkFlowApprovedFormChangeds { get; set; } = new List<DynamicFormWorkFlowApprovedFormChanged>();
        public string? DelegateApproveAllUserName { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public long? DelegateApproveAllUserId { get; set; }
        public string? DynamicFormName { get; set; }
        public string? DynamicFormSectionName { get; set; }
        public int? SequenceNo { get; set; }
        public long? DynamicFormId { get; set; }
        public bool? IsAllowDelegateUser { get; set; } = false;
        public bool? IsParallelWorkflow { get; set; } = false;
        public bool? IsAnomalyStatus {  get; set; } = false;
    }
}
