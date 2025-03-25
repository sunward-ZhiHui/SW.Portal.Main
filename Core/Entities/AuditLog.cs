using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AuditLog
    {
        [Key]
        public long AuditLogId { get; set; }
        public string? AuditType { get; set; }
        public string? TableName { get; set; }
        public bool? IsPrimaryKey { get; set; }
        public string? PrimaryKeyName { get; set; }
        public long? PrimaryKeyValue { get; set; }
        public bool? IsForeignKey { get; set; }
        public string? ForeignKeyName { get; set; }
        public string? ColumnName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public bool? IsModified { get; set; }
        public DateTime? AuditDate { get; set; }
        public long? AuditByUserId { get; set; }
        [NotMapped]
        public string? AuditByUser { get; set; }
        [NotMapped]
        public dynamic? NewData { get; set; }
        [NotMapped]
        public dynamic? OldData { get; set; }
        [NotMapped]
        public string? DataType { get; set; }
        [NotMapped]
        public string? ColumnNames { get; set; }
        [NotMapped]
        public string? ReferencedColumnName { get; set; }
        [NotMapped]
        public string? OldValueName { get; set; }
        [NotMapped]
        public string? NewValueName { get; set; }
    }
}
