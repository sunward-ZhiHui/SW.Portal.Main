using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFCAPproval:BaseEntity
    {
        [Key]
        public long CCFCAPprovalID { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsNotApproved { get; set; }

        public string? Comments { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
