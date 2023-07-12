using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AddressResponse
    {
        public long AddressID { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public int? AddressType { get; set; }
        public int? PostCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public int? OfficePhone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }
}
