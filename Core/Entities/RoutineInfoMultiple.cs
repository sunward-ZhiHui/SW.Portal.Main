using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RoutineInfoMultiple
    {
        [Key]
        public long RoutineInfoMultipleId { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public long? RoutineInfoId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? AcitivityMasterName { get; set; }
    }
}
