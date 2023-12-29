using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TempSalesPackInformation
    {
        [Key]
        public long TempSalesPackInformationId { get; set; }
        public long? FinishproductGeneralInfoId { get; set; }
        public long? FinishProductGeneralInfoLineId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedByUser { get; set; }
        [NotMapped]
        public string? ModifiedByUser { get; set; }
    }
}
