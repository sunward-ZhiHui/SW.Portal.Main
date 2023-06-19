using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppConsumptionEntry:BaseEntity
    {
        public long ConsumptionEntryID { get; set; }
        public string TransferFrom { get; set; }
        public long TransferFromID { get; set; }
        public string TransferTo { get; set; }
        public long TransferToID { get; set; }
        public string ProdOrderNo { get; set; }
        public string ReplanRefNo { get; set; }
        public string SubLotNo { get; set; }
     
        public int LineCount { get; set; }
     
        public string Description { get; set; }
        public bool? IsNewEntry { get; set; }

        public string Company { get; set; }
    }
}
