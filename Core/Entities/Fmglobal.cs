using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Fmglobal
    {
        [Key]
        public long FmglobalId { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? ExpectedShipmentDate { get; set; }
        public string Pono { get; set; }
        public long? LocationToId { get; set; }
        public long? LocationFromId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? FmglobalStausId { get; set; }
        public long? SoCustomerId { get; set; }
        public long? SoCustomerShipingAddressId { get; set; }

    }
}
