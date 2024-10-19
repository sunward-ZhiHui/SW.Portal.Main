using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ApplicationMasterChildModel
    {
        
        public long ID { get; set; }
        public long Index { get; set; }
        public long ApplicationMasterChildId { get; set; }
        public long? ApplicationMasterParentId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Value { get; set; }
        public string? Label { get; set; }
        public string? Description { get; set; }
        public long? ParentId { get; set; }
        public Guid? SessionId { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ApplicationMasterName { get; set; }
        public string? ParentName { get; set; }
        public string? StatusCode { get; set; }
        public List<ApplicationMasterChildModel> Children { get; set; } = new List<ApplicationMasterChildModel>();
    }
}
