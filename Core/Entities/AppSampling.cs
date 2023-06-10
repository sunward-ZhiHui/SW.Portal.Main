using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AppSampling
    {
        //public AppSampling()
        //{
        //    AppSamplingLine = new HashSet<AppSamplingLine>();
        //}

        public int SamplingId { get; set; }
        public string ScanDocument { get; set; }
        public string TicketNo { get; set; }
        public string SublotNo { get; set; }
        public string BatchNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Company { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
    }
}
