using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductionActivityCheckedDetailsModel : BaseModel
    {
        public long ProductionActivityCheckedDetailsId { get; set; }
        public long? ProductionActivityAppLineId { get; set; }
        public long? ProductionActivityAppId { get; set; }
        public int? ActivityInfoId { get; set; }
        public long? ActivityStatusId { get; set; }
        public long? ActivityResultId { get; set; }
        public bool? IsCheckNoIssue { get; set; } = false;
        public long? CheckedById { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string? CheckedComment { get; set; }
        public bool? IsCheckReferSupportDocument { get; set; } = false;
        public byte[]? CommentImage { get; set; }
        public string? CommentImageType { get; set; }
        public string? CommentImages { get; set; }
        public string? ActivityStatusName { get; set; }
        public string? ActivityResultName { get; set; }
        public string? CheckedByUserName { get; set; }
    }
    public class ProductionActivityRoutineCheckedDetailsModel : BaseModel
    {
        public long ProductionActivityRoutineCheckedDetailsId { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public long? ProductionActivityRoutineAppId { get; set; }
        public int? ActivityInfoId { get; set; }
        public long? RoutineStatusId { get; set; }
        public long? RoutineResultId { get; set; }
        public bool? IsCheckNoIssue { get; set; } = false;
        public long? CheckedById { get; set; }
        public DateTime? CheckedDate { get; set; }
        public string? CheckedComment { get; set; }
        public bool? IsCheckReferSupportDocument { get; set; } = false;
        public byte[]? CommentImage { get; set; }
        public string? CommentImageType { get; set; }
        public string? RoutineStatusName { get; set; }
        public string? RoutineResultName { get; set; }
        public string? CheckedByUserName { get; set; }
        public string? CommentImages { get; set; }
    }
    public class ProductionActivityRoutineEmailModel : BaseModel
    {
        public Guid? ActivityRoutineEmailSessionId { get; set; }
        public Guid?  EmailTopicSessionId { get; set;  }
      public long?  ProductionActivityRoutineAppLineID { get; set; }
    }
}
