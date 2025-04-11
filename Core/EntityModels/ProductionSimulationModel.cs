using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductionSimulationModel : BaseModel
    {
        public long ProductionSimulationId { get; set; }
        public string? ProdOrderNo { get; set; }
        public long ItemId { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? PackSize { get; set; }
        public decimal Quantity { get; set; }
        public decimal PlannedQuantity { get; set; }
        public string? Uom { get; set; }
        public decimal PerQuantity { get; set; }
        public string? PerQtyUom { get; set; }
        public string? BatchNo { get; set; }
        public DateTime StartingDate { get; set; }
        public string? ItemName { get; set; }
        public bool? IsOutput { get; set; } = false;
        public string? RePlanRefNo { get; set; }
        public string? BatchSize { get; set; }
        public string? Dispense { get; set; }

    }
}
