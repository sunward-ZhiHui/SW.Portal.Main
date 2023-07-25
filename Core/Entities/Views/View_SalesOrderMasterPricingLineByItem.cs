using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_SalesOrderMasterPricingLineByItem
    {
        public long SalesOrderMasterPricingLineId { get; set; }
        public long? SalesOrderMasterPricingId { get; set; }
        public long? ItemId { get; set; }
        public long? SellingMethodId { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? SmAllowPriceByPercent { get; set; }
        public decimal? SmAllowPriceBy { get; set; }
        public long? SmAllowPriceId { get; set; }
        public decimal? NewAllowPrice { get; set; }
        public bool? IsSalesManagerApprovePrice { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string SellingMethodName { get; set; }
        public string SmAllowPriceName { get; set; }
        public string Description { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public string No { get; set; }
        public long? CompanyId { get; set; }
        public long? SalesPricingForId { get; set; }
        public DateTime PriceValidaityFrom { get; set; }
        public DateTime PriceValidaityTo { get; set; }
        public long? ReasonForChangeId { get; set; }
        public string MasterType { get; set; }
        public string PlantCode { get; set; }
        public string CompanyName { get; set; }
        public string NavCompanyName { get; set; }
        public string ReasonForChangeName { get; set; }
        public string SalesPricingForName { get; set; }
        public string NavCompany { get; set; }
        public List<SalesOrderMasterPricingLineSellingMethod> SalesOrderMasterPricingLineSellingMethods { get; set; }
    }
}
