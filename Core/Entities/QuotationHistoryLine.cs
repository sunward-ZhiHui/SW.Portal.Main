using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class QuotationHistoryLine
    {
        public long QuotationHistoryLineId { get; set; }
        public long? QutationHistoryId { get; set; }
        [Required(ErrorMessage = "Product is Required")]
        public long? ProductId { get; set; }
        [Required(ErrorMessage = "Source is Required")]
        public string? Source { get; set; }
        public decimal? Quantity { get; set; }
        public long? Uomid { get; set; }
        public long? PackingId { get; set; }
        public long? OfferCurrencyId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal? OfferPrice { get; set; }
        public long? OfferUomid { get; set; }
        public decimal? Focqty { get; set; }
        public long? ShippingTermsId { get; set; }
        public bool? IsTenderExceed { get; set; }
        public Guid? SessionId { get; set; }
        public bool? IsAwarded { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Remarks { get; set; }
        public string? Packing { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductDescription2 { get; set; }
        public string? ProductUom { get; set; }
        public string? OfferCurrency { get; set; }
        public string? OfferUom { get; set; }
        public string? ShippingTerms { get; set; }
        public string? StatusCode { get; set; }
        public string? AddedBy { get; set; }
        [Required(ErrorMessage = "Quantity is Required")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? Quantitys { get; set; }
        [Range(1, Int64.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? Focqtys { get; set; }
    }
}
