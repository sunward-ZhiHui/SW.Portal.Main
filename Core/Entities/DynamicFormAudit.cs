using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormAudit
    {
        public long DynamicFormAuditId { get; set; }
        public long? DynamicFormId { get; set; }
        public long? DynamicFormSetId { get; set; }
        public string? FormType { get; set; }
        public string? Type { get; set; }
        public string? PreValue { get; set; }
        public string? CurrentValue { get; set; }
        public Guid? SessionId { get; set; }
        public long? AuditUserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? AuditDate { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string? ColumnName { get; set; }
        public string? AuditUser { get; set; }
        public string? DynamicFormName { get; set; }
        public int? RowIndex { get; set; }
        public string? DisplayName { get; set; }
        public bool? IsFormDeleted { get; set; } = false;
        public long? DynamicFormSectionId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public string? FormDisplayType { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public int? RowItemIndex { get; set; }
        public List<DynamicFormAudit> DynamicFormAudits { get; set; } = new List<DynamicFormAudit>();
    }
}
