using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFDImplementationDetails 
    {
        [Key]
        public long CCFDImplementationDetailsID { get; set; }
        public long CCFDImplementationID { get; set; }
        public long ClassOFDocumentID { get; set; }
        public bool? IsRequired { get; set; }
        public long? ResponsibiltyTo { get; set; }
        public long? DoneBy { get; set; }
        public DateTime? DoneByDate { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }

    }
}
