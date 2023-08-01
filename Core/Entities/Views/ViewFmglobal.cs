using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewFmglobal
    {
        public long? FmglobalId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        [Required(ErrorMessage = "Shipment Date is Required")]
        public DateTime? ExpectedShipmentDate { get; set; }
        
        public string Pono { get; set; }
        [Required(ErrorMessage = "Location To is Required")]
        public long? LocationToId { get; set; }
        [Required(ErrorMessage = "Location From is Required")]
        public long? LocationFromId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public long? FmglobalStausId { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PlantCode { get; set; }
        public string PlantDescription { get; set; }
        public string NavCompany { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string FmglobalStatus { get; set; }
        public long? SoCustomerId { get; set; }
        public long? SoCustomerShipingAddressId { get; set; }
    }
}
