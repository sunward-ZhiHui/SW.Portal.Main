using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormReport
    {
        [Key]
        public long DynamicFormReportId { get; set; }
        public long? DynamicFormId { get; set; }
        public bool? IsDeleted { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public Guid? SessionId { get; set; }
        public string? FilePath { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        public byte[]? FileData { get; set; }
        [NotMapped]
        public Guid? DynamicFormSessionId { get; set; }
    }
}
