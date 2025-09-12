using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
}
