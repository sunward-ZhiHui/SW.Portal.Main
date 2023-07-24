using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NavItemBatchNo
    {
        [Key]
        public long NavItemBatchNoId { get; set; }
        public long? ItemId { get; set; }
        public string ItemNo { get; set; }
        public string BatchNo { get; set; }
        public string BatchSize { get; set; }
        public string Description { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
