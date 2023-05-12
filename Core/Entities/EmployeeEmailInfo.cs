using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
