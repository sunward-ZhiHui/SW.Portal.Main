using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppSamplingLine
    {
        public int AppSamplingLineId { get; set; }
        public int? AppSamplingId { get; set; }
        public int? SamplingPurposeId { get; set; }
        public string DrumNo { get; set; }
        public string QcsampleNo { get; set; }
        public int? Qty { get; set; }
        public int? Uom { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
    }
}
