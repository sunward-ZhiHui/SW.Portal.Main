using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ActivityMasterMultiple
    {
        [Key]
        public long ActivityMasterMultipleId { get; set; }
        public long? AcitivityMasterID { get; set; }
        public long? ProductionActivityAppLineId { get; set; }
    }
}
