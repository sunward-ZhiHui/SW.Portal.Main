using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationMasterParent
    {
        public long ApplicationMasterParentId { get; set; }
        public long ApplicationMasterParentCodeId { get; set; }
        [Required(ErrorMessage ="Name Is Required")]
        public string ApplicationMasterName { get; set; }
        [Required(ErrorMessage = "Description Is Required")]
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public bool? IsDisplay { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public long? ApplicationMasterChildId { get; set; }
        public string? ApplicationMasterName2 { get; set; }
        public string? Description2 { get; set; }
        public string? ApplicationMasterName3 { get; set; }
        public string? Description3 { get; set; }
        public long? ApplicationMasterParentId2 { get; set; }
        public long? ApplicationMasterParentCodeId2 { get; set; }
        public long? ApplicationMasterParentId3 { get; set; }
        public long? ApplicationMasterParentCodeId3 { get; set; }
    }
}
