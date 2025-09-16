using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentViewer.Models
{
    public class PlanningForProductionProcessByMachine
    {
        public long PlanningForProductionProcessByMachineId { get; set; }
        public long? TypeOfProductionId { get; set; }
        public string? SquenceForPlanning { get; set; }
        public long? ProductionPlanningProcessId { get; set; }
        public long? TypeOfProcessId { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? ProductionPlanningProcess { get; set; }
    }
}
