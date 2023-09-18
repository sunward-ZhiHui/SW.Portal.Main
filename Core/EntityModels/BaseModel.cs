using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class BaseModel
    {
        public int? StatusCodeID { get; set; }
        [DisplayName("Status")]
        public string StatusCode { get; set; }
        public Nullable<System.DateTime> LastAccessDate { get; set; }
        public long? AddedByUserID { get; set; }
        [DisplayName("Created By")]
        public string AddedByUser { get; set; }
        [DisplayName("Created Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss tt}")]
        public DateTime? AddedDate { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        [DisplayName("Modified Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy HH:mm:ss tt}")]
        public DateTime? ModifiedDate { get; set; }
        public string Errormessage { get; set; }
        public bool IsError { get; set; }
        public bool IsRecordExist { get; set; }
        public string CompanyName { get; set; }
        public long? DocumentID { get; set; }
        public bool SaveVersionData { get; set; } = false;

        public Guid? SessionId { get; set; }
        public string ScreenID { get; set; }
        public string ReferenceInfo { get; set; }
        public string DepartmentCompanyName { get; set; }

        public long? DeleteByUserID { get; set; }
        public string? DeleteByUser { get; set; }
       
        public DateTime? DeleteByDate { get; set; }
    }
}
