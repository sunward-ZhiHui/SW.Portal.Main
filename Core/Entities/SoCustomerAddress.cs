using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SoCustomerAddress
    {
        [Key]
        public long SoCustomerAddressId { get; set; }
        public long? CustomerId { get; set; }
        public long? AddressId { get; set; }
        public string? AddressType { get; set; }
        public bool isBilling { get; set; }  
        public bool isShipping { get; set;}      

    }
}
