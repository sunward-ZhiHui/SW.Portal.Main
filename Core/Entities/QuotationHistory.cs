using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class QuotationHistory
    {
        [Key] 
        public long QuotationHistoryId { get; set; }
        public long? CompanyId { get; set; }
        public string? SwreferenceNo { get; set; }
        public DateTime? Date { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerRefNo { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        [NotMapped]
        public string? PlantCode { get; set; }
        [NotMapped]
        public string? CompanyDescription { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public long? SoCustomerId { get; set; }
        [NotMapped]
        public string? ShipCode { get; set; }
        [NotMapped]
        public string? CustomerName { get; set; }
        [NotMapped]
        public string? AssignToRep { get; set; }
        [NotMapped]
        public string? Address1 { get; set; }
        [NotMapped]
        public string? Address2 { get; set; }
        [NotMapped]
        public string? City { get; set; }
        [NotMapped]
        public string? StateCode { get; set; }
        [NotMapped]
        public string? PostCode { get; set; }
        [NotMapped]
        public string? Channel { get; set; }
        [NotMapped]
        public string? Type { get; set; }

    }
}
