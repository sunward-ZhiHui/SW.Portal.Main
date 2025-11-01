using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeEmailInfo
    {
        [Key]
        public long EmployeeEmailInfoID { get; set; }
        public long? EmployeeID { get; set; }
        public long? SubscriptionID { get; set; }
        public long? EmailGuideID { get; set; }
        [NotMapped]
        public long? AddedByUserId { get; set; }
        [NotMapped]
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public long? ModifiedByUserId { get; set; }
        [NotMapped]
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? EmailGuide {  get; set; }
        [NotMapped]
        public string? Subscription { get; set; }
    }
}
