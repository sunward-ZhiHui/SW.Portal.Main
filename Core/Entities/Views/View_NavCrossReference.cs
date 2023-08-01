using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_NavCrossReference
    {
        public long NavCrossReferenceId { get; set; }
        public long? ItemId { get; set; }
        public string? TypeOfCompany { get; set; }
        public long? CompanyId { get; set; }
        public long? NavVendorId { get; set; }
        public long? NavCustomerId { get; set; }
        public string? CrossReferenceNo { get; set; }
        public string? VendorName { get; set; }
        public string? VendorItemNo { get; set; }
        public string? VendorNo { get; set; }
        public string? NAVCustomerName { get; set; }
        public string? NAVCustomerCode { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
        public string? CustomerName { get; set; }
        public string? ShipCode { get; set; }
        public long? SoCustomerId { get; set; }

    }
}
