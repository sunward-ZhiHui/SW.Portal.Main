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
    public class Address
    {
        [Key]
        public long AddressID { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? AddressType { get; set; }
        public int? PostCode { get; set; }  
        public string? City { get; set;}
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public int? OfficePhone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set;}
        [NotMapped]
        public long? CustomerId { get; set;}
        [NotMapped]
        public long SoCustomerAddressId { get; set;}
        [NotMapped]
        public bool isBilling { get; set; }
        [NotMapped]
        public bool isShipping { get; set; }

    }
}
