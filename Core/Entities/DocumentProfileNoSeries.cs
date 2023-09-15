using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentProfileNoSeries
    {
        [Key]
        public long ProfileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string Abbreviation1 { get; set; }
        public string Abbreviation2 { get; set; }
        public bool? AbbreviationRequired { get; set; }
        public string SpecialWording { get; set; }
        public string StartingNo { get; set; }
        public bool? StartWithYear { get; set; }
        public int? NoOfDigit { get; set; }
        public int? IncrementalNo { get; set; }
        public bool? TranslationRequired { get; set; }
        public DateTime? LastCreatedDate { get; set; }
        public string LastNoUsed { get; set; }
        public string Note { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CompanyId { get; set; }
        public long? DeparmentId { get; set; }
        public long? GroupId { get; set; }
        public string GroupAbbreviation { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryAbbreviation { get; set; }
        public bool? IsGroupAbbreviation { get; set; }
        public bool? IsCategoryAbbreviation { get; set; }
        public int? SeperatorToUse { get; set; }
        public int? LinkId { get; set; }
        public int? ProfileTypeId { get; set; }
        public string SampleDocumentNo { get; set; }
        public bool? IsEnableTask { get; set; }
    }
}
