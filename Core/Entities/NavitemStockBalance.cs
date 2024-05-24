using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
