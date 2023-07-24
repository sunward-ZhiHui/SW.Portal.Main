using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewFmglobal
    {
        public long? FmglobalId { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? ExpectedShipmentDate { get; set; }
        public string Pono { get; set; }
        public long? LocationToId { get; set; }
        public long? LocationFromId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? FmglobalStausId { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PlantCode { get; set; }
        public string PlantDescription { get; set; }
        public string NavCompany { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string FmglobalStatus { get; set; }
    }
}
