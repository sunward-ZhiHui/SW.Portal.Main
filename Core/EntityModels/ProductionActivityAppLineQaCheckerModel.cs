using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductionActivityAppLineQaCheckerModel
    {
        public long ProductionActivityAppLineQaCheckerId { get; set; }
        public long? ProductionActivityAppLineId { get; set; }
        public bool? QaCheck { get; set; }
        public string QaCheckFlag { get; set; }
        public long? QaCheckUserId { get; set; }
        public DateTime? QaCheckDate { get; set; }
        public string QaCheckUser { get; set; }
    }
}
