using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ACEntryModel : BaseModel
    {
        public long ACEntryId { get; set; }
        [Required(ErrorMessage = "From Date is Required")]
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        [Required(ErrorMessage = "Customer is Required")]
        public long? CustomerId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public string? CustomerName { get; set; }
        public long? DocumentId { get; set; }
        public string? FileName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Remark { get; set; }
        public string? Version { get; set; }
        public long? ACEntryCopyId { get; set; }
        public List<ACEntryLinesModel>? Acentrylines { get; set; } = new List<ACEntryLinesModel>();
    }
    public class ACEntryLinesModel : BaseModel
    {
        public long ACEntryLineId { get; set; }
        public long? ACEntryId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal? Quantity { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public string? BUOM { get; set; }
        public int? PackSize { get; set; }
        public string? Packuom { get; set; }
        public string? ItemCategory { get; set; }
        public string? AcItemNo { get; set; }
        public string? VendorNo { get; set; }
        public string? DistName { get; set; }
        public string? GenericCode { get; set; }

    }
}
