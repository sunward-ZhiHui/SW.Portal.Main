using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GenericCodes
    {
        public long GenericCodeId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Description2 { get; set; }
        public string? ProfileNo { get; set; }
        public string? PackingCode { get; set; }
        public long? Uom { get; set; }
        public long? ManufacringCountry { get; set; }
        public long? ProductNameId { get; set; }
        public long? PackingUnitsId { get; set; }
        public long? SupplyToId { get; set; }
        public bool? IsSimulation { get; set; }
        [NotMapped]
        public string? UomName { get; set; }
    }
}
