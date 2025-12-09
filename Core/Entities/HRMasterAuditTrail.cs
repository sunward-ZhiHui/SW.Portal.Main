using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class HRMasterAuditTrail
    {

        public long HRMasterAuditTrailID { get; set; }
        public long? HRMasterSetId { get; set; }
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
        public int? RowIndex1 { get; set; }
        public string? DisplayName { get; set; }
        public string? HeaderDisplayName { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public List<HRMasterAuditTrail?> HRMasterAuditTrailItems { get; set; } = new List<HRMasterAuditTrail?>();
        public string? PermissionPrevDisplayName { get; set; }
        public string? PermissionCurrentDisplayName { get; set; }
    }
}
