using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NavprodOrderLine
    {
        [Key]
        public long NavprodOrderLineId { get; set; }
        public string? Status { get; set; }
        [Required(ErrorMessage = "Prod OrderNo is Required")]
        public string? ProdOrderNo { get; set; }
        public int? OrderLineNo { get; set; }
        [Required(ErrorMessage = "Item No is Required")]
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public string? UnitofMeasureCode { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncUserId { get; set; }
        [Required(ErrorMessage = "Ticket No is Required")]
        public string? RePlanRefNo { get; set; }
        [Required(ErrorMessage = "Batch No is Required")]
        public string? BatchNo { get; set; }
        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime? StartDate { get; set; }
        public decimal? OutputQty { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public string? TopicId { get; set; }
        [NotMapped]
        public string? ProdOrderNoDesc { get; set; }
        public string? LocationCode { get; set; }
        public string? ProductionBOMNo { get; set; }
        public string? CompanyName { get; set; }
    }
}