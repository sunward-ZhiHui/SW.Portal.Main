using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentViewer.Models
{
    public class PlanningForProductionProcessByMachineRelated
    {
        public long PlanningForProductionProcessByMachineRelatedId { get; set; }
        public long? PlanningForProductionProcessByMachineId { get; set; }
        public long? FixAssetMachineNameRequipmentId { get; set; }
        public long? AssetIDWithModelId { get; set; }
        [NotMapped]
        public string? PlanningForProductionProcessByMachine { get; set; }
        public string? AssetIDWithModel { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
