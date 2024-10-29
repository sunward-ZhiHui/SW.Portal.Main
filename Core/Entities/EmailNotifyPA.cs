using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailNotifyPA
    {
        [Key]
        public long ID { get; set; }
        public string? Name { get; set; }
        public string? Page { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "is Required")]
        public long? NotifyTypeID { get; set; }
        public long? AddedByUserID { get; set; }
        public string? AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? EmailCreatedDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public Guid? SessionId { get; set; }
        public string? CategoryName { get; set; }
        public string? Comment { get; set; }

        public bool? IsDelete { get; set; }
    }
}
