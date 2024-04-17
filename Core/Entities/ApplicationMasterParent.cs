using System;
using System.Collections.Generic;
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
        public string ApplicationMasterName { get; set; }
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
    }
}
