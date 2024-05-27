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
    public class NavitemStockBalance:BaseEntity
    {
        [Key]
        public long NavStockBalanceId { get; set; }
        public long ItemId { get; set; }
        public DateTime StockBalMonth { get; set; }
        public int StockBalWeek { get; set; }
        public decimal Quantity { get; set; }
        public decimal RejectQuantity { get; set;}
        public decimal ReworkQty { get; set;}
        public decimal WIPQty { get; set; }
        public decimal GlobalQty { get; set;}
        public decimal KIVQty { get; set;}
        public decimal SupplyWIPQty { get; set; }
        public decimal Supply1ProcessQty { get;set;}
        [NotMapped]
        public long CompanyID { get; set; }
        [NotMapped]
        public DateTime? FilterDate { get; set; }
        [NotMapped]
        public int WeekNumberOfMonth { get; set; }
        [NotMapped]
        public string ItemNo { get; set; }
        [NotMapped]
        public string ItemDescription { get; set; }
        [NotMapped]
        public string ItemDescription2 { get; set; }
        [NotMapped]
        public string InternalRef { get; set; }
        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string UOM { get; set; }
        [NotMapped]
        public string PackSize { get; set;}
        [NotMapped]
        public string PackUOM { get; set;}
        [NotMapped]
        public string NavStatusCodeID { get; set;}
        [NotMapped]
        public int Month { get; set; }
        [NotMapped]
        public int Year { get; set; }
    }
}
