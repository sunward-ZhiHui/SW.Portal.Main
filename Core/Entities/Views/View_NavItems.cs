using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_NavItems
    {
        public long ItemId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public string No { get; set; }
        public string RelatedItemNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string ItemType { get; set; }
        public decimal? Inventory { get; set; }
        public string InternalRef { get; set; }
        public string ItemRegistration { get; set; }
        public string ExpirationCalculation { get; set; }
        public string BatchNos { get; set; }
        public string ProductionRecipeNo { get; set; }
        public bool? Qcenabled { get; set; }
        public string SafetyLeadTime { get; set; }
        public string ProductionBomno { get; set; }
        public string RoutingNo { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? UnitPrice { get; set; }
        public string VendorNo { get; set; }
        public string VendorItemNo { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemTrackingCode { get; set; }
        public string Qclocation { get; set; }
        public string Company { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncBy { get; set; }
        public long? CategoryId { get; set; }
        public bool? Steroid { get; set; }
        public string ShelfLife { get; set; }
        public string Quota { get; set; }
        public int? PackSize { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? PackUom { get; set; }
        public long? NAVFPDescription { get; set; }   
        public string? NAVFPDescriptionName { get; set; }
        public long? PackSizeBUOM { get; set; }
        public string? PackSizeBUOMName { get; set; }
        public string? Recommendedplanning { get; set; }
        public long? GenericCodeId { get; set; }
        public bool? IsDifferentAcuom { get; set; }
        public decimal? PackQty { get; set; }
        public string PurchaseUom { get; set; }
        public long? ReplenishmentMethodId { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsPortal { get; set; }
        [Required(ErrorMessage = "Item SerialNo is Required")]
        public string ItemSerialNo { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
        public int? CodeId { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public long? PackSizeId { get; set; }
        public long? SupplyToId { get; set; }
        public long? UomId { get; set; }
        public string? PackSizeName { get; set; }
        public string? SupplyToName { get; set; }
        public string? UomName { get; set; }
        public string? GenericCode { get; set; }
        public string? MethodCode { get; set; }
        public string? SteroidName { get; set; }
        public long? MethodCodeId { get; set; }
        public IEnumerable<long?> NavItemCustomerItemID { get; set; } = new List<long?>();
    }
    public class NavPackingMethodModel : BaseModel
    {
        public long PackingMethodId { get; set; }
        public long? ItemId { get; set; }
        public long? NoOfUnitsPerShipperCarton { get; set; }
        public long? PalletSize { get; set; }
        public string? PalletSizeName { get; set; }
        public long? NoOfShipperCartorPerPallet { get; set; }
    }
}
