using DevExpress.CodeParser;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DocumentViewer.Models
{
    public class ProductionPlanningScheduler
    {
        [JsonPropertyName("ProductionPlanningSchedulerId")]
        public long ProductionPlanningSchedulerId { get; set; }
        [JsonPropertyName("Text")]
        public string? Text { get; set; }
        [JsonPropertyName("Description")]
        public string? Description { get; set; }
        [JsonPropertyName("StartDate")]
        public DateTime? StartDate { get; set; }
        [JsonPropertyName("EndDate")]
        public DateTime? EndDate { get; set; }
        [NotMapped]
        [JsonPropertyName("ProductionPlanningProcess")]
        public long? ProductionPlanningProcess { set; get; }
        [NotMapped]
        [JsonPropertyName("PlanningForProductionProcessByMachine")]
        public long? PlanningForProductionProcessByMachine { set; get; }
        public string? ReplanRefNo { set; get; }
        public string? ProdOrderNo { set; get; }
        public string? RecipeNo { set; get; }
        public string? ProductionBOMNo { set; get; }
        public string? BatchNo { set; get; }
        public string? ItemNo { set; get; }
        public string? Description2 { set; get; }
        public string? InternalRef { set; get; }
        public string? LocationCode { set; get; }
        public decimal? Quantity { set; get; }
        public string? MachineCenterName { set; get; }
        public string? Remarks { set; get; }
        public string? SubStatus { set; get; }
        public DateTime? OutputDate { set; get; }
        public string? NotToContinue { set; get; }
        public string? CompleteOrder { set; get; }
        public string? UnitofMeasureCode { set; get; }
        public decimal? RemainingQuantity { set; get; }
        public decimal? FinishedQuantity { set; get; }
        public long? PlanningForProductionProcessByMachineRelatedId { set; get; }
        [NotMapped]
        public int Progress { get; set; } = 0;
        [NotMapped]
        public long? ParentId { get; set; }
        [NotMapped]
        public long? Id { get; set; }
        [NotMapped]
        public string? Type { get; set; }
    }
}
