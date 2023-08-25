using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFEClosure:BaseEntity
    {
        [Key]
        public long? CCFEClosureID { get;set; }
        public bool? IsSatisfactory { get; set; }
        public bool? IsNotSatisfactory { get; set; }
        public string? Comments { get; set; }
        public long? ClosedBy { get; set; }
        public DateTime Date { get;set; }


    }
}
