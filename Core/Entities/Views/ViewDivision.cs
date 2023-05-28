using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewDivision
    {
        [Key]
        public long DivisionID { get; set; }
        public int Index { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        [Required(ErrorMessage = "Code is Required")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }
        public string? PlantCode { get; set; }
        public string? PlantDescription { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string? Description { get; set; }
        public int? CodeId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }

        public string? DivisionDrop { get; set; }
    }
}
