using Core.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DynamicFormReportItems
    {
        public string? AttrId { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public bool? IsGrid { get; set; } = false;
        public Guid? DynamicFormSessionId { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormDataGridSessionId { get; set; }
        public Guid? DynamicFormSectionGridAttributeSessionId { get; set; }
        public long? DynamicGridFormId { get; set; }
        public long? DynamicGridFormDataId { get; set; }
        public long? DynamicGridFormDataGridId { get; set; }
        public long? DynamicFormSectionGridAttributeId { get; set; }
        public string? Url { get; set; } = string.Empty;
        public List<DynamicFormDataResponse>? GridItems { get; set; } = new List<DynamicFormDataResponse>();
        public bool? IsSubForm { get; set; } = false;
        public List<dynamic>? GridSingleItems { get; set; } = new List<dynamic>();

    }
    public class DynamicFormWorkFlowFormReportItems
    {
        public long DynamicFormWorkFlowFormId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? AssignedUserId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? SequenceNo { get; set; }
        public bool? IsAllowDelegateUserForm { get; set; } = false;
        public bool? IsPendingApproval { get; set; } = false;
        public bool? IsDelegateUser { get; set; } = false;
        public long? DelegateUserId { get; set; }
        public long? CurrentApprovalUserId { get; set; }
        public string? AssignedUser { get; set; }
        public string? DelegateUser { get; set; }
        public string? CurrentApprovalUserName { get; set; }
        public string? SectionName { get; set; }
        public List<DynamicFormWorkFlowFormApprovedReportItems> DynamicFormWorkFlowFormApprovedReportItems { get; set; } = new List<DynamicFormWorkFlowFormApprovedReportItems>();
    }
    public class DynamicFormWorkFlowFormApprovedReportItems
    {
        public long DynamicFormWorkFlowApprovedFormId { get; set; }
        public long? DynamicFormWorkFlowFormID { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedDescription { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? SequenceNo { get; set; }
        public bool? IsDelegateUser { get; set; } = false;
        public bool? IsPendingApproval { get; set; } = false;
        public long? AssignedUserId { get; set; }
        public long? DelegateUserId { get; set; }
        public long? CurrentApprovalUserId { get; set; }
        public string? AssignedUser { get; set; }
        public string? DelegateUser { get; set; }
        public string? CurrentApprovalUserName { get; set; }
    }
    public class DynamicFormEmailSubCont
    {
        public long DynamicFormEmailSubContId { get; set; }
        public string? AttrId { get; set; }
        public string? TypeName { get; set; }
        public int? SortBy { get; set; }
        public Guid? DynamicFormSessionID { get; set; }
        public string? LabelName { get; set; }
        public string? ValueName { get; set; }
    }
}
