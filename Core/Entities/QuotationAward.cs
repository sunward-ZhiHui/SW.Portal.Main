using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class QuotationAward
    {
        [Key]
        public long QuotationAwardId { get; set; }
        [Required(ErrorMessage = "Type Of Order is Required")]
        public long? TypeOfOrderId { get; set; }
        [Required(ErrorMessage = "Navision No is Required")]
        public long? ItemId { get; set; }
        public decimal? TotalQty { get; set; }
        public bool IsCommitment { get; set; } = false;
        public decimal? CommittedQty { get; set; }
        public DateTime? CommittedOn { get; set; }
        public bool IsFollowDate { get; set; } = false;
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? QuotationHistoryId { get; set; }
        public decimal? Price { get; set; }
        public string? ReasonForDifferentQty { get; set; }
        public string? ReasonForDifferentPrice { get; set; }
        public bool IsDifferentQty { get; set; } = false;
        public bool IsDifferentPrice { get; set; } = false;
        [NotMapped]
        public string? TypeOfOrder { get; set; }
        public string? GenericCode { get; set; }
        [NotMapped]
        public string? GenericCodeDescription { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? CustomerRefNo { get; set; }
        [NotMapped]
        public string? IsCommitmentBy { get; set; } = "No";
        [NotMapped]
        public string? IsFollowDateBy { get; set; } = "No";
        [QuotationAwardCustomValidation]
        public string? TotalQtys { get; set; }
        [Range(1, Int64.MaxValue, ErrorMessage = "Only Number allowed")]
        public int? CommittedQtys { get; set; }
        [QuotationAwardDifferentPriceCustomValidation]
        public string? Prices { get; set; }
    }
}
