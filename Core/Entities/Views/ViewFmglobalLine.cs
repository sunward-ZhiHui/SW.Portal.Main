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
        public string TempPalletNo { get; set; }
        public long? PalletEntryNoId { get; set; }
        public long? LocationToId { get; set; }
        public long? LocationFromId { get; set; }
        public int? IsHandQty { get; set; }
        public long? FmglobalLinePreviousId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? TransactionUserId { get; set; }
        public string? TransactionUser { get; set; }
        public int? TransactionQty { get; set; }
        public long? FmglobalHeaderId { get; set; }
        public long? FmglobalMoveId { get; set; }
        public DateTime? TransactionModifiedDate { get; set; }
        public long? TransactionModifiedUserId { get; set; }
        public string? TransactionModifiedUser { get; set; }

    }
}
