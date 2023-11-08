using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionSimulation
    {
        [Key]
        public long ProductionSimulationId { get; set; }
        public long? CompanyId { get; set; }
        public string ProdOrderNo { get; set; }
        public long ItemId { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string PackSize { get; set; }
        public decimal Quantity { get; set; }
        public string Uom { get; set; }
        public decimal PerQuantity { get; set; }
        public string PerQtyUom { get; set; }
        public string BatchNo { get; set; }
        public decimal? PlannedQty { get; set; }
        public decimal? OutputQty { get; set; }
        public bool? IsOutput { get; set; }
        public DateTime StartingDate { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ProcessDate { get; set; }
        public bool? IsBmrticket { get; set; }
        public string RePlanRefNo { get; set; }
        public string BatchSize { get; set; }
        public string Dispense { get; set; }
    }
}
