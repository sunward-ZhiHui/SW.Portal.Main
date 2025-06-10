using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class MasterBlanketOrderModel : BaseModel
    {
        public long MasterBlanketOrderId { get; set; }
        public long? CompanyID { get; set; }
        public DateTime? FromPeriod { get; set; }
        public DateTime? ToPeriod { get; set; }
        public long? CustomerID { get; set; }
        public bool? IsRequireVersionInformation { get; set; }
        public Guid? VersionSessionId { get; set; }
        public List<MasterBlanketOrderLineModel> MasterBlanketOrderLineModels { get; set; }=new List<MasterBlanketOrderLineModel>();
        public string? CustomerName { get; set; }

    }
    public class MasterBlanketOrderLineModel : BaseModel
    {
        public long MasterBlanketOrderLineID { get; set; }
        public long? MasterBlanketOrderId { get; set; }
        public int? NoOfWeeksPerMonth { get; set; }
        public DateTime? WeekOneStartDate { get; set; }
        public long? WeekOneLargestOrder { get; set; }
        public DateTime? WeekTwoStartDate { get; set; }
        public long? WeekTwoLargestOrder { get; set; }
        public DateTime? WeekThreeStartDate { get; set; }
        public long? WeekThreeLargestOrder { get; set; }
        public DateTime? WeekFourStartDate { get; set; }
        public long? WeekFourLargestOrder { get; set; }
        public long? BalanceQtyForCalculate { get; set; }


    }
}
