using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionActivityPermission : BaseEntity
    {
        public long ProductActivityPermissionId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public string Type { get; set; }
        public bool? IsChecker { get; set; }
        public bool? IsUpdateStatus { get; set; }
        public bool? IsCheckOut { get; set; }
        public bool? IsMail { get; set; }
        public bool? IsSupportDocuments { get; set; }
        public bool? IsCopyLink { get; set; }
        public bool? IsViewHistory { get; set; }
        public bool? IsNonCompliance { get; set; }
        public bool? IsActivityInfo { get; set; }
        public bool? IsViewFile { get; set; }
        public long? ProductActivityCaseResponsDutyId { get; set; }
        public long? UserID { get; set; }
        public long? UserGroupID { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
