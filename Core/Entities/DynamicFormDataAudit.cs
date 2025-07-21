using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormDataAudit
    {
        public long DynamicFormDataAuditId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public string? AttributeId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public string? PrevValueId { get; set; }
        public string? CurrentValueId { get; set; }
        public string? PreValue { get; set; }
        public string? CurrentValue { get; set; }
        public DateTime? AuditDateTime { get; set; }
        public long? AuditUserId { get; set; }
        public string? CurrentData { get; set; }
        public string? PrevData { get; set; }
        public long? PreUserId { get; set; }
        public DateTime? PreUpdateDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? DynamicFormId { get; set; }
        public string? AuditUser { get; set; }
        public string? PreUser { get; set; }
        public long? RowNum { get; set; }
        public string? DisplayName {  get; set; }  
        public Guid? DynamicFormSessionId {  get; set; }
        public bool? IsDeleted { get; set; }=false;
        public string? DynamicFormName { get; set; }
        public string? ProfileNo { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public long? CountData { get; set; }
        [NotMapped]
        public List<DynamicFormDataAudit> DynamicFormDataAudits { get; set; }=new List<DynamicFormDataAudit>();
    }
}
