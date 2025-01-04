using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ReleaseProdOrderLine
    {
        [Key]
        public long ReleaseProdOrderLineId { get; set; }
        public string? ItemNo { get; set; }
        public string? ProdOrderNo { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public string? BatchNo { get; set; }
        public string? BatchSize { get; set; }
        public decimal? Quantity { get; set; }
        public string? LocationCode { get; set; }
        public string? MachineCenterCode { get; set; }
        public string? Remarks { get; set; }
        public string? ReplanRefNo { get; set; }
        public bool? Promised { get; set; } = false;
        public DateTime? StartingDate { get; set; }
        public string? Status { get; set; }
        public string? SubStatus { get; set; }
        public string? UnitOfMeasureCode { get; set; }
        public long? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? PrePrintedStartDate { get; set; }
        public bool? ProduceExactQuantity { get; set; } = false;
        public long? ItemId { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? BatchNos { get; set; }
    }
}
