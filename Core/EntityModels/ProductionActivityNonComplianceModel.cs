using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductionActivityNonComplianceModel : BaseModel
    {
        public long ProductionActivityNonComplianceId { get; set; }
        public long? ProductionActivityAppLineId { get; set; }
        public long? IpirReportId { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public string? Type { get; set; }
        public IEnumerable<long> UserIDs { get; set; }=new List<long>();
        public string? ActionType { get; set; }
        public long? ProductionActivityPlanningAppLineId { get; set; }
        public string? Notes { get; set; }
        public List<ProductionActivityNonComplianceUserModel> ProductionActivityNonComplianceUserModels { get; set; }=new List<ProductionActivityNonComplianceUserModel>();
    }
    public class ProductionActivityNonComplianceUserModel
    {
        public long ProductionActivityNonComplianceUserId { get; set; }
        public long? ProductionActivityNonComplianceId { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Notes { get; set; }
    }
}
