using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RawMatPurch
    {
        [Key]
        public long RawMatPurchId { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public string? UnitOfMeasureCode { get; set; }
        public long? ItemId { get; set; }
        public long? CompanyId { get; set; }
        public string? BatchNo { get; set; }
        public string? QcRefNo { get; set; }
    }
}
