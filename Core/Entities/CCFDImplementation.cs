using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFDImplementation:BaseEntity
    {
        [Key]
        public long? CCFDImplementationID { get; set;}
        public long? ClassOfDocumentID { get; set; }
        public string? HODComments { get; set; }
        public string? HODSignature { get; set; }
        public DateTime HODDate { get; set;}
        public bool? IsAcceptable { get; set;}
         public bool? IsNotAcceptable { get; set;}
        public long? VerifiedBy { get; set; }
        public DateTime VerifiedDate { get; set; }



    }
}
