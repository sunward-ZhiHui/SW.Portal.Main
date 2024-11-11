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
    public class UserGroup
    {
        [Key]
        public long UserGroupId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        [UserGroupNameCustomValidation]
        public string? Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsTms { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]

        public string? StatusCode { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Users is Required")]
        [SelectUserGropupUserCustomValidation]
        public IEnumerable<long?> SelectUserIDs { get; set; } = new List<long?>();
    }
}
