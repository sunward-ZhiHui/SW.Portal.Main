using Core.Entities.Base;
using Core.EntityModels;
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
        [NotMapped]
        public int WeekNumberOfMonth { get; set; }
        [NotMapped]
        public int Month { get; set; }
        [NotMapped]
        public int Year { get; set; }

    }
    public class NavItemStockBalanceModel : BaseModel
    {
        public long NavStockBalanceId { get; set; }
        public long? ItemId { get; set; }
        public DateTime? StockBalMonth { get; set; }
        public int? StockBalWeek { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? RejectQuantity { get; set; }
        public decimal? ReworkQty { get; set; }
        public decimal? Wipqty { get; set; }
        public decimal? GlobalQty { get; set; }
        public string? DistName { get; set; }
        public string? ItemName { get; set; }
    }
    public class NavStockBalanceModel : BaseModel
    {
        public long AvnavStockBalID { get; set; }
        public DateTime? StockBalMonth { get; set; }
        public int? StockBalWeek { get; set; }
        public string? ItemDecs { get; set; }
        public string? Uom { get; set; }
        public decimal? RemainingQty { get; set; }
        public bool? IsNav { get; set; }
        public string? PackSize { get; set; }
        public int? Ac { get; set; }

        public string? Dist { get; set; }
        public string? ItemCode { get; set; }
        public string? Packuom { get; set; }

        public string? SWItemNo { get; set; }
        public string? SWDesc { get; set; }
        public string? SWDesc2 { get; set; }
        public long? CompanyId { get; set; }
        public string? InternalRefNo { get; set; }
        public string? ItemGroup { get; set; }
    }
    public class ACImportModel
    {
        public string? DistName { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? QtyOnHand { get; set; }
        public string? Location { get; set; }
        public string? Month { get; set; }
        public string? StockBalDate { get; set; }
    }
}
