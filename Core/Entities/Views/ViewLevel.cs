using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewLevel
    {
        [Key]
        public long LevelID { get; set; }
        public int Index { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyID { get; set; }
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
        public string? CompanyName { get; set; }
        [Required(ErrorMessage = "Division is Required")]
        public long? DivisionID { get; set; }
        public string? DivisionName { get; set; }
        public string? PlantCode { get; set; }
    }
}
