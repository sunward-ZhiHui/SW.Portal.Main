using Core.Entities.Base;
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
    public class DocumentRole
    {
        [Key]
        public long DocumentRoleId { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        [DocumentRoleCustomValidation]
        public string DocumentRoleName { get; set; }
        public string DocumentRoleDescription { get; set; }
        [Required(ErrorMessage = "Status Code is Requred")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
    }
}
