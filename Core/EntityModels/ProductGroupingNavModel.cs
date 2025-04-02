using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductGroupingNavModel : BaseModel
    {
        public long ProductGroupingNavId { get; set; }
        public long? ProductGroupingManufactureId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        public string? VarianceNo { get; set; }
        public string? NavNo { get; set; }
        public string? ManufactureBy { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public string? InternalRef { get; set; }
        public string? ManufactureFor { get; set; }
        public string? UOM { get; set; }
        public long? ReplenishmentMethodId { get; set; }
        public string? ReplenishmentMethod { get; set; }
        public string? ItemCategoryCode { get; set; }
        public long? GenericCodeSupplyToMultipleId { get; set; }
        public string? GenericCodeSupplyToMultiple { get; set; }
        [Required(ErrorMessage = "Stock Depletion Status is Required")]
        public int? StockDepletionStatusId { get; set; }
    }
}
