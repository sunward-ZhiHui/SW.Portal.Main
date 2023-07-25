using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ItemBatchInfo
    {
        public long ItemBatchId { get; set; }
        public long ItemId { get; set; }
        public long? CompanyId { get; set; }
        public string LocationCode { get; set; }
        public string BatchNo { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public decimal? QuantityOnHand { get; set; }
        public decimal? NavQuantity { get; set; }
        public decimal? IssueQuantity { get; set; }
        public decimal? BalanceQuantity { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal? StockOutBatchConfirm { get; set; }
    }
}
