using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModels;
namespace Core.Entities
{
    public class Memo
    {
        public long MemoId { get; set; }
        [Required(ErrorMessage = "Subject is Required")]
        public string? Subject { get; set; }
        public string MemoContent { get; set; } = "";
        public bool? IsAttachment { get; set; } = false;
        public Guid? SessionId { get; set; }
        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Status is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? UserType { get; set; }
        public IEnumerable<long?> SelectUserIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectUserGroupIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectLevelMasterIDs { get; set; } = new List<long?>();
        public List<MemoUser> MemoUserList { get; set; } = new List<MemoUser>();
        public string? UserNameLists { get; set; }
        public string? AcknowledgeUserNameLists { get; set; }
        public IEnumerable<long?> AcknowledgeUserIDs { get; set; } = new List<long?>();
        public IEnumerable<long?> SelectCCUserIDs { get; set; } = new List<long?>();
        public bool? IsAcknowledgement { get; set; } = false;
        public long? MemoUserId { get; set; }
        public List<DocumentsModel> DocumentsData { get; set; } = new List<DocumentsModel>();
        public string? StatusCode { get; set; }
        public DateTime? AcknowledgementDate { get; set; }
        public string? AddedByUser { get; set; }
        public string? ModifiedByUser { get; set; }
    }
}
