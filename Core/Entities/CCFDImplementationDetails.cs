using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFDImplementationDetails :BaseEntity
    {
        [Key]
        public long CCFDImplementationDetailsID { get; set; }
        public long CCFDImplementationID { get; set; }
        public long ClassOFDocumentID { get; set; }
        public bool? IsRequired { get; set; }
        public long? ResponsibiltyTo { get; set; }
        public long? DoneBy { get; set; }
        public DateTime? DoneByDate { get; set; }


    }
}
