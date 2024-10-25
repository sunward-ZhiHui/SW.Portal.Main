using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FinishedProdOrderLine
    {
        public long FinishedProdOrderLineId { get; set; }
        public string? Status { get; set; }
        [Required(ErrorMessage = "Prod OrderNo is Required")]
        public string? ProdOrderNo { get; set; }
        public int? OrderLineNo { get; set; }
        [Required(ErrorMessage = "Item No is Required")]
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        [Required(ErrorMessage = "Replan RefNo is Required")]
        public string? ReplanRefNo { get; set; }
        public DateTime? StartingDate { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        [Required(ErrorMessage = "OptStatus is Required")]
        public string? OptStatus { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        [NotMapped]
        public string? CompanyCode { get; set; }
    }
    public class FinishedProdOrderLineOptStatus
    {
        public string? OptStatus { get; set; }
    }
}
