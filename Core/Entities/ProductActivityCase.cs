using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductActivityCase
    {
        [Key]
        public long ProductActivityCaseId { get; set; }
        public long? ManufacturingProcessId { get; set; }
        public string Instruction { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? ManufacturingProcessChildId { get; set; }
        public long? CategoryActionId { get; set; }
        public long? ActionId { get; set; }
        public int? ProductionTypeId { get; set; }
        public string Reference { get; set; }
        public string CheckerNotes { get; set; }
        public string Upload { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? ImplementationDate { get; set; }
        public string IsLanguage { get; set; }
        public string ReferenceMalay { get; set; }
        public string CheckerNotesMalay { get; set; }
        public string UploadMalay { get; set; }
        public string ReferenceChinese { get; set; }
        public string CheckerNotesChinese { get; set; }
        public string UploadChinese { get; set; }
        public long? VersionCodeStatusId { get; set; }
        public string VersionNo { get; set; }
        public bool? IsEnglishLanguage { get; set; }
        public bool? IsMalayLanguage { get; set; }
        public bool? IsChineseLanguage { get; set; }
    }
}
