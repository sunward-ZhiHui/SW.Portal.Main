using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class EmployeeEmailInfoResponse
    {
        public long EmployeeEmailInfoID { get; set; }
        public long? EmployeeID { get; set; }
        public long? SubscriptionID { get; set; }
        public long? EmailGuideID { get; set; }

        public string Subscription { get; set; }
        public string EmailGuide { get; set; }
    }
}
