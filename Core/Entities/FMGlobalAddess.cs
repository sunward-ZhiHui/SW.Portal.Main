using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FMGlobalAddess
    {
        [Key]
        public long FmglobalAddressId { get; set; }
        public long? FmglobalId { get; set; }
        public long? AddressId { get; set; }
        public string? AddressType { get; set; }
        public bool? isBilling { get; set; }
        public bool? isShipping { get; set; }
    }
}
