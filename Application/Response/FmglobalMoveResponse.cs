using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class FmglobalMoveResponse
    {
        public long FmglobalMoveId { get; set; }
        public long? LocationID { get; set; }
        public long? FmglobalLineId { get; set; }
        public int? IsMoveQty { get; set; }
        public IEnumerable<long?> PalletMoveEntryNoIds { get; set; }
    }
}
