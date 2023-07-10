using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SoCustomer
    {
        [Key]
        public long SoCustomerId { get; set; }
        public string ShipCode { get; set; }
        public string CustomerName { get; set; }
        public string AssignToRep { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostCode { get; set; }
        public string Channel { get; set; }
    }
}
