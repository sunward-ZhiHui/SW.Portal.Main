using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_ApplicationMasterDetail
    {
        public long ApplicationMasterDetailID { get; set; }
        public int Index { get; set; }
        public long? ApplicationMasterID { get; set; }
        public string? Description { get; set; }
        public string? ApplicationMasterName { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Value { get; set; }
        public long? ApplicationMasterCodeID { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ProfileId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public string? StatusCode { get; set; }
    }
}
