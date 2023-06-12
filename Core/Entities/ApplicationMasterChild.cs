using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationMasterChild
    {
        public long ApplicationMasterChildId { get; set; }
        public long? ApplicationMasterParentId { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        public long? ParentId { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
