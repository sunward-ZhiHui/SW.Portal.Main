using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FmglobalLineItemResponse
    {
        public long FmglobalLineItemId { get; set; }
        public long? FmglobalLineId { get; set; }
        public long? ItemId { get; set; }
        public long? BatchInfoId { get; set; }
        public long? CartonPackingId { get; set; }
        public int? NoOfCarton { get; set; }
        public int? NoOfUnitsPerCarton { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
    }
}
