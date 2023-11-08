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
    public class NavprodOrderLine
    {
        [Key]
        public long NavprodOrderLineId { get; set; }
        public string Status { get; set; }
        public string ProdOrderNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Description1 { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public string UnitofMeasureCode { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncUserId { get; set; }
        public string RePlanRefNo { get; set; }
        public string BatchNo { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? OutputQty { get; set; }
        public long? CompanyId { get; set; }
        public string TopicId { get; set; }
    }
}