using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ReceiveEmail: BaseModel
    {
        public long ReceiveEmailId { get; set; }
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public int? EmailSalesStatusId { get; set; }
        public bool? IsAcknowledgement { get; set; }
        public string Sonumber { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string NotRelatedDescription { get; set; }
        public long? SameAsSalesOrderCompleteProcessFlag { get; set; }
        public bool? SameAsSalesOrderCompleteProcess { get; set; }
        public long? SalesOrderId { get; set; }
        public string HtmlFileName { get; set; }
        public string Description { get; set; }
        public long? EmailCategoryId { get; set; }
        public long? DocumentId { get; set; }
        public Guid? SessionId { get; set; }
        public long? FileProfileTypeId { get; set; }

        public virtual ApplicationUser AddedByUser { get; set; }
        public virtual Documents Document { get; set; }
        public virtual ApplicationMasterDetail EmailCategory { get; set; }
        public virtual CodeMaster EmailSalesStatus { get; set; }
        //public virtual FileProfileType FileProfileType { get; set; }
        public virtual ApplicationUser ModifiedByUser { get; set; }
        //public virtual SalesOrder SalesOrder { get; set; }
    }
}
