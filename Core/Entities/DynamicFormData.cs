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
    public class DynamicFormData : BaseEntity
    {
        public long DynamicFormDataId { get; set; }
        public string? DynamicFormItem { get; set; }
        public bool? IsSendApproval { get; set; } = false;
        public long? DynamicFormId { get; set; }
        public Guid? FileProfileSessionID { get; set; }

        [NotMapped]
        public AttributeHeaderListModel? AttributeHeader { get; set; }
        [NotMapped]
        public object? ObjectData { get; set; }
        [NotMapped]
        public string? DynamicFormCurrentItem { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? ScreenID { get; set; }
        [NotMapped]
        public bool? IsApproval { get; set; }
        [NotMapped]
        public bool? IsApproved { get; set; }
        [NotMapped]
        public string? ApprovalStatus { get; set; }
        [NotMapped]
        public int? ApprovalStatusId { get; set; }
        [NotMapped]
        public string? RejectedUser { get; set; }
        [NotMapped]
        public DateTime? RejectedDate { get; set; }
        [NotMapped]

        public long? RejectedUserId { get; set; }
        [NotMapped]
        public string? PendingUser { get; set; }
        [NotMapped]
        public long? PendingUserId { get; set; }
        [NotMapped]
        public string? ApprovedUser { get; set; }
        [NotMapped]
        public DateTime? ApprovedDate { get; set; }
        [NotMapped]
        public long? ApprovedUserId { get; set; }
        [NotMapped]
        public List<DynamicFormApproved> DynamicFormApproved { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
        [NotMapped]
        public long? FileProfileTypeId { get; set; }
        [NotMapped]
        public string? FileProfileTypeName { get; set; }
        [NotMapped]
        public int? isDocuments { get; set; } = 0;
        [NotMapped]
        public int? IsFileprofileTypeDocument { get; set; } = 0;
        [NotMapped]
        public string? CurrentUserName { get; set; }
        [NotMapped]
        public long? CurrentUserId { get; set; }
    }
}
