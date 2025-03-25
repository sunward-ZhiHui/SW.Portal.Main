using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class NavCustomerModel
    {
        public long CustomerId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ResponsibilityCenter { get; set; }
        public string? LocationCode { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public string? County { get; set; }
        public string? CountryRegionCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Contact { get; set; }
        public string? SalespersonCode { get; set; }
        public string? CustomerPostingGroup { get; set; }
        public string? GenBusPostingGroup { get; set; }
        public string? VatbusPostingGroup { get; set; }
        public string? PaymentTermsCode { get; set; }
        public string? CurrencyCode { get; set; }
        public string? LanguageCode { get; set; }
        public string? ShippingAdvice { get; set; }
        public string? ShippingAgentCode { get; set; }
        public decimal? BalanceLcy { get; set; }
        public decimal? BalanceDueLcy { get; set; }
        public decimal? SalesLcy { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncBy { get; set; }
        public int? StatusCodeId { get; set; }
        public string? Company { get; set; }
        public string? CustomerName { get; set; }
        public long? CompanyId { get; set; }
    }
}
