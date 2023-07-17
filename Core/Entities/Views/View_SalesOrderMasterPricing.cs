using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_SalesOrderMasterPricing
    {
        public long SalesOrderMasterPricingId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        [Required(ErrorMessage = "SalesPricing For is Required")]
        public long? SalesPricingForId { get; set; }
        [Required(ErrorMessage = "Price Validaity From is Required")]
        public DateTime PriceValidaityFrom { get; set; }
        [Required(ErrorMessage = "Price Validaity To is Required")]
        public DateTime PriceValidaityTo { get; set; }
        public long? ReasonForChangeId { get; set; }
        public string MasterType { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string PlantCode { get; set; }
        public string CompanyName { get; set; }
        public string NavCompanyName { get; set; }
        public string ReasonForChangeName { get; set; }
        public string SalesPricingForName { get; set; }
        public string NavCompany { get; set; }
    }
}
