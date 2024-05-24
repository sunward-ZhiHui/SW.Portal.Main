using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DistStockBalance:BaseEntity
    {
        [Key]
        public long DistStockBalanceId { get; set; }
        public long DistItemId {  get; set; }
        public DateTime StockBalMonth { get; set; }
        public int StockBalWeek { get; set; }
        public decimal POQuantity { get; set; }
        public decimal Avg6MonthQty { get; set; }
        public decimal Quantity { get; set;}
        [NotMapped]
        public long CompanyID { get; set; }
        [NotMapped]
        public DateTime? FilterDate { get; set; }
        [NotMapped]
        public string Dist { get; set; }
        [NotMapped]
        public string DistDescription { get; set; }
        [NotMapped]
        public string NavItem {  get; set; }
        [NotMapped]
        public string NavDescription { get; set; }
        [NotMapped]
        public string NavDescription2 { get; set; }
        [NotMapped]
        public string InternalRef { get; set;}
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string PackUOM { get; set; }
        [NotMapped]
        public string PackSize { get; set; }
        [NotMapped]
        public string NavItemNo { get; set;}
        [NotMapped]
        public string UOM { get; set;}
        [NotMapped]
        public string NavCompany { get; set; }
        public int WeekNumberOfMonth { get; set; }

    }
}
