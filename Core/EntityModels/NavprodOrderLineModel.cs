using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class NavprodOrderLineModel : BaseModel
    {
        public long NavprodOrderLineId { get; set; }
        public string? Status { get; set; }
        public string? ProdOrderNo { get; set; }
        public string? RePlanRefNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? OutputQty { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public string? UnitofMeasureCode { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncUserId { get; set; }
        public string? Name { get; set; }
        public string? TopicId { get; set; }
        public long? CompanyId { get; set; }
        public string? Uom { get; set; }
        public string? InternalRef { get; set; }
        public decimal? PackQty { get; set; }
        public string? BatchNos { get; set; }
        public string? BatchSize { get; set; }
    }
    public class NavprodOrderLineItemsModel
    {
        public List<ProductionSimulation> ProductionSimulation { get; set; } = new List<ProductionSimulation>();
        public List<Navitems> Navitems { get; set; } = new List<Navitems>();
        public List<NavprodOrderLine> NavprodOrderLine { get; set; } = new List<NavprodOrderLine>();
    }
}
