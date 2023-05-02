using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewDesignation
    {
        [Key]
        public long DesignationID { get; set; }
        public long CompanyID { get; set; }
        public string? Name { get; set; }
        public long? LevelID { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? HeadCount { get; set; }
        public string? CompanyName { get; set; }
        public int? CodeId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public string? LevelName { get; set; }
    }
}
