using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailActivityCatgorys
    {
        [Key]
        public long ID { get; set; }
        public long TopicId { get; set; }
        [Required]
        public string Name { get; set; }
        public long? GroupTag { get; set; }
        public long? CategoryTag { get; set; }
        public long? ActionTag { get; set; }
        public string? GroupName { get; set; }
        public string? CategoryName { get; set; }
        public string? ActionName { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
    }
}
