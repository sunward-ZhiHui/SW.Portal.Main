using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public long? DelegateUserIds { get; set; }
        public string? DelegateUserName { get; set; }
        public long? ActualUserId { get; set; }
        public string? ActualUserName { get; set; }
        public string? DynamicFormName { get; set; }
        public string? ProfileNo { get; set; }
        public string? LockedUser { get; set; }
        public long? LockedUserId { get; set; }
        public bool? IsLocked { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public bool? IsDraft { get; set; }
        [NotMapped]
        public int? IsFileprofileTypeDocument { get; set; } = 0;
        public Guid? DynamicFormSessionID { get; set; }
        public Guid? DynamicFormDataSessionID { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? DynamicFormId { get; set; }
        public int? DynamicFormWorkFlowFormTotalCount { get; set; } 
        public int? DynamicFormWorkFlowFormCount { get; set; }
        public string? WorkFlowStatus{ get; set; }
        public int? FlowStatusID { get; set; }
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
