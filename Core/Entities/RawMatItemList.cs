using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RawMatItemList
    {
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public decimal? Inventory { get; set; }
        public string? InternalRef { get; set; }
        public string? ItemRegistration {  get; set; }
        public string? BatchNos { get;set; }   
        public string? PSOItemNo { get;set; }
        public string? ProductionRecipeNo { get; set; }
        public string? SafetyLeadTime { get; set; }
        public string? ProductionBOMNo { get; set; }
        public string? RoutingNo { get; set; }
        public string? BaseUnitofMeasure { get; set; }
        public decimal? StandardCost { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? LastDirectCost { get; set; }
        public string? ItemCategoryCode { get; set; }
        public string? ProductGroupCode { get; set; }
        public long? CompanyId { get; set;}
        public string? Type { get; set;}

    }
}
