using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class DivisionResponse
    {
        public long DivisionID { get; set; }
        public long? CompanyId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
        public string? Description { get; set; }
        public int? CodeId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
    }
}
