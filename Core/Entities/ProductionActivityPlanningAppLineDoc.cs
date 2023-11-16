using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionActivityPlanningAppLineDoc
    {
        [Key]
        public long ProductionActivityPlanningAppLineDocId { get; set; }
        public long? ProductionActivityPlanningAppLineId { get; set; }
        public long? DocumentId { get; set; }
        public string? Type { get; set; }
        public long? IpirReportId { get; set; }
        public Guid? SessionId { get; set; }
    }
}
