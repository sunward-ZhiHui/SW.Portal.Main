using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class GenericCodesModel : BaseModel
    {
        public long GenericCodeID { get; set; }
        [Required(ErrorMessage = "Code is Required")]
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? PackingCode { get; set; }
        public long? Uom { get; set; }
        public string? GenericUom { get; set; }
        public long? ManufacringCountry { get; set; }
        public IEnumerable<long?> ManufacturingIds { get; set; } = new List<long?>();
        public List<long?> SellingToIds { get; set; } = new List<long?>();
        public IEnumerable<long?> SupplyToIds { get; set; } = new List<long?>();
        public List<string> SupplyToNames { get; set; } = new List<string>();
        public long? ProductNameId { get; set; }
        public string? ProductName { get; set; }
        public long? PackingUnitsId { get; set; }
        public string? PackingUnits { get; set; }
        public long? SupplyToId { get; set; }
        public string? SupplyTo { get; set; }
        public bool? IsSimulation { get; set; } = false;
        public string? Name { get; set; }
        public string? UomName { get; set; }
    }
    public class GenericCodeCountry
    {
        [Key]
        public long GenericCodeCountryId { get; set; }
        public long? CountryId { get; set; }
        public long? GenericCodeId { get; set; }
        public bool IsSellingToCountry { get; set; } = false;
    }
    public partial class GenericCodeSupplyToMultiple
    {
        [Key]
        public long GenericCodeSupplyToMultipleId { get; set; }
        public long? GenericCodeId { get; set; }
        public long? SupplyToId { get; set; }
        public string? GenericCodeSupplyDescription { get; set; }
    }
    public partial class CompanyListingCustomerCode
    {
        [Key]
        public long CompanyListingCustomerCodeId { get; set; }
        public long? CompanyListingId { get; set; }
        public long? CustomerCodeId { get; set; }
        public string? CustomerCode { get; set; }
    }
    public class ProductGroupingManufactureModel : BaseModel
    {
        [Key]
        public long ProductGroupingManufactureId { get; set; }
        public long? ProductGroupingId { get; set; }
        [Required(ErrorMessage = "Manufacture By is Required")]
        public long? ManufactureById { get; set; }
        public string? ManufactureBy { get; set; }
        public long? SupplyToId { get; set; }
        public string? SupplyTo { get; set; }
        public long? DosageFormId { get; set; }
        public long? DrugClassificationId { get; set; }

        public string? DosageFormName { get; set; }
        public string? DrugClassificationName { get; set; }
    }
}
