using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PlanningForProductionProcessByMachine : BaseModel
    {
        public long PlanningForProductionProcessByMachineId { get; set; }
        [Required(ErrorMessage = "Type of Planning is required")]
        public long? TypeOfProductionId { get; set; }
        public string? SquenceForPlanning { get; set; }
        [Required(ErrorMessage = "Production Planning Process is required")]
        public long? ProductionPlanningProcessId { get; set; }
        [Required(ErrorMessage = "Machine or Human related is required")]
        public long? TypeOfProcessId { get; set; }
        [NotMapped]
        public string? TypeOfProduction { get; set; }
        [NotMapped]
        public string? ProductionPlanningProcess { get; set; }
        [NotMapped]
        public string? TypeOfProcess { get; set; }
    }
    public class PlanningForProductionProcessByMachineRelated : BaseModel
    {
        public long PlanningForProductionProcessByMachineRelatedId { get; set; }
        public long? PlanningForProductionProcessByMachineId { get; set; }
        [Required(ErrorMessage = "NAV Machine Code is required")]
        public long? FixAssetMachineNameRequipmentId { get; set; }
        //[Required(ErrorMessage = "Dropdown for FA is required")]
        public long? AssetIDWithModelId { get; set; }
        public string? PlanningForProductionProcessByMachine { get; set; }
        public string? AssetIDWithModel { get; set; }
    }
    public class PlanningForProductionProcessByMachineRelatedItem
    {
        [Required(ErrorMessage = "Dropdown for FA is required")]
        public object? AssetIDWithModelId { get; set; }
    }
    public class ResourceData : AppointmentData
    {
        public string? Text { get; set; }
        public int? Id { get; set; }
        public int? GroupId { get; set; }
        public string? Color { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public long? ProductionPlanningSchedulerId { get; set; }
        public long? PlanningForProductionProcessByMachine { set; get; }
        public string Type { get; set; }
    }
    public class AppointmentData
    {
        public string? Subject { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Nullable<bool> IsAllDay { get; set; }
        public string CategoryColor { get; set; }
        public string RecurrenceRule { get; set; }
        public Nullable<int> RecurrenceID { get; set; }
        public Nullable<int> FollowingID { get; set; }
        public string RecurrenceException { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
    }
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
