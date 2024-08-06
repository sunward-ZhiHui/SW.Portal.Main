using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class JobScheduleFun
    {
        public long JobScheduleFunId { get; set; }
        public long? JobScheduleFunUniqueId { get; set; }
        public string? ScheduleFunctionName { get; set; }
        public string? DisplayName { get; set; }
    }
}
