using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormWorkFlowApprovedForm
    {
        public long DynamicFormWorkFlowApprovedFormID { get; set; }
        public long? DynamicFormWorkFlowFormID { get; set; }
        public long? UserID { get; set; }
        public int? SortBy { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedDescription { get; set; }
        public long? ApprovedByUserID { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByUser { get; set; }
        public string? UserName { get; set; }
        public long? DynamicFormDataID { get; set; }
        public string? ApprovedStatus { get; set; }
        public long? DynamicFormWorkFlowID { get; set; }
        public long? DelegateUserId { get; set; }
        public string? DelegateUserName { get; set; }
        public long? ActualUserId { get; set; }
        public string? ActualUserName { get; set; }
    }
    public class DynamicFormWorkFlowApprovedFormChanged
    {
        public long DynamicFormWorkFlowApprovedFormChangedID { get; set; }
        public long? DynamicFormWorkFlowApprovedFormID { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public bool? IsApprovedStatus { get; set; }
    }
}
