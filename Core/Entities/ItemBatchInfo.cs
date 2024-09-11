using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ItemBatchInfo
    {
        [Key]
        public long ItemBatchId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public string? LocationCode { get; set; }
        [Required(ErrorMessage = "BatchNo is Required")]
        public string? BatchNo { get; set; }
        public string? LotNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public decimal? QuantityOnHand { get; set; }
        public decimal? NavQuantity { get; set; }
        public decimal? IssueQuantity { get; set; }
        public decimal? BalanceQuantity { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? StockOutBatchConfirm { get; set; }
        public string? ItemNo { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemDescription2 { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
    }
}
