using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class view_SoCustomerAddress
    {
        [Key]
        public long AddressID { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? AddressType { get; set; }
        public string? CountryName { get; set; }
        public string? StateName { get; set; } 
        public string? CityName { get; set; }
        public long? CountryID { get; set; }
        public long? StateID { get; set; }
        public long? CityID { get; set; }
        public int? PostCode { get; set; }      
        public int? OfficePhone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        [NotMapped]
        public long? CustomerId { get; set; }
        [NotMapped]
        public long SoCustomerAddressId { get; set; }
        [NotMapped]
        public bool? isBilling { get; set; }
        [NotMapped]
        public bool? isShipping { get; set; }
        [NotMapped]
        public string? PostCodeNumber { get; set; }
    }
}
