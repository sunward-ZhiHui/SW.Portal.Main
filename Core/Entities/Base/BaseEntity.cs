using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Base
{
    public class BaseEntity
    {
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        [NotMapped]
        public int Index { get; set; }

    }
}
