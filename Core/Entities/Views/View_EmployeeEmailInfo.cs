using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_EmployeeEmailInfo
    {
        public long EmployeeEmailInfoID { get; set; }
        public long? EmployeeID { get; set; }
        [Required(ErrorMessage = "Subscription is Required")]
        public long? SubscriptionID { get; set; }
        [Required(ErrorMessage = "Email Guide is Required")]
        public long? EmailGuideID { get; set; }

        public string Subscription { get; set; }
        public string EmailGuide { get; set; }
        [NotMapped]
        public long? AddedByUserId { get; set; }
        [NotMapped]
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public long? ModifiedByUserId { get; set; }
        [NotMapped]
        public DateTime? ModifiedDate { get; set; }
    }
}
