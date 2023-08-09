using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FmglobalMove
    {
        [Key]
        public long FmglobalMoveId { get; set; }
        public long? LocationID { get; set; }
        public long? FmglobalLineId { get; set; }
        public int? IsHandQty { get; set; }
        public long? FmglobalLinePreviousId { get; set; }
        public long? LocationToID { get; set; }
        public long? FMGlobalID { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? TransactionQty { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
