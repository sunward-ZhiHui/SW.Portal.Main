using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionActivityAppLine:BaseEntity
    {
        [Key]
        public long ProductionActivityAppLineID { get; set; }
        public long? ProductionActivityAppID { get; set; }
        public string ActionDropdown  { get; set; }
        public long? ProdActivityActionID { get;set; }
        public long? ProdActivityCategoryID { get;set; }
        public long? ManufacturingProcessID { get; set; }
        public bool IsTemplateUpload { get;set; }

       public long? ProductActivityCaseLineID { get; set; }
        public long? NavprodOrderLineID { get; set; }
        public string Comment { get; set; }
        public bool QaCheck { get; set; }
        public bool IsOthersOptions { get; set;}
        public long? ProdActivityResultID { get; set;}
        public long? ManufacturingProcessChildID { get; set; }
        public long? ProdActivityCategoryChildID { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public string TopicID { get; set; }
        public long? QaCheckUserID { get; set; }
        public DateTime? QaCheckDate { get; set; }
      public long? LocationID { get; set; }
        public long? ProductActivityCaseID { get; set;}
        public long? ActivityMasterID { get; set; }
        public long? ActivityStatusID { get; set; }
        public string CommentImage { get; set; }
        public string CommentImageType { get; set; }
        [NotMapped]
        public string? Process { get; set; }
        [NotMapped]
        public string? Result { get; set;}
        [NotMapped]
        public string? Category { get; set; }
        [NotMapped]
        public string? Action { get; set; }
        [NotMapped]
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public Guid? ActivityEmailSessionId { get; set; }
        [NotMapped]
        public Guid? EmailTopicSessionId { get; set; }
    }
}
