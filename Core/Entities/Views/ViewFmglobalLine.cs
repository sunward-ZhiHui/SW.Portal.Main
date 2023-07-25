using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewFmglobalLine
    {
        public long FmglobalLineId { get; set; }
        public long? FmglobalId { get; set; }
        public long? PalletEntryId { get; set; }
        public string PalletNoYear { get; set; }
        public long? PalletNoAuto { get; set; }
        public string PalletNo { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PalletEntryName { get; set; }
    }
}
