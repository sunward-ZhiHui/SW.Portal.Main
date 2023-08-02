using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NavCrossReference
    {
        [Key]
        public long NavCrossReferenceId { get; set; }
        public long? ItemId { get; set; }
        public string? TypeOfCompany { get; set; }
        public long? CompanyId { get; set; }
        public long? NavVendorId { get; set; }
        public long? NavCustomerId { get; set; }
        public string? CrossReferenceNo { get; set; }
        public long? SoCustomerId { get; set; }
    }
}
