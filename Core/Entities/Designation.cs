using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Designation
    {
        [Key]
        public long DesignationId { get; set; }
        public long? LevelId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? HeadCount { get; set; }
        public long? CompanyId { get; set; }
        public long? SubSectionTid { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public string? SubSectionName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? LevelName { get; set; }
    }
}
