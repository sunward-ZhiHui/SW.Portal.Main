using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TempSalesPackInformationFactor
    {
        public long TempSalesPackInformationFactorId { get; set; }
        public long? TempSalesPackInformationId { get; set; }
        [Required(ErrorMessage = "Sales Factor is Required")]
        public int? SalesFactor { get; set; }
        [Required(ErrorMessage = "Profile is Required")]
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? Fpname { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        [Required(ErrorMessage = "Qty Pack Per Carton is Required")]
        public long? QtypackPerCarton { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? SmallestQtyUnit { get; set; }
        [NotMapped]
        public string? SmallestPerPack { get; set; }
        [NotMapped]
        public string? ItemName { get; set; }
        [NotMapped]
        public string? ItemDescription { get; set; }
        [NotMapped]
        public string? ProfileName { get; set; }
        [NotMapped]
        public string? QTYPackPerCartonName { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        public int? SellingPackUnit { get; set; }
        public int? SalesPackSize { get; set; }
    }
}
