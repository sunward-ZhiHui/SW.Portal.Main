using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentProfileNoSeriesModel
    {
        public long ProfileID { get; set; }
        [Required(ErrorMessage = "Profile Name is Required")]
        [DocumentProfileNoSeriesValidation]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [DocumentProfileNoSeriesAbbreviationValidation]
        public string? Abbreviation { get; set; }
        public string? Abbreviation1 { get; set; }
        public string? Abbreviation2 { get; set; }
        public bool? AbbreviationRequired { get; set; } = false;
        public string? SpecialWording { get; set; }
        [Required(ErrorMessage = "Starting No is Required")]
        [StartingNosValidation]
        public string? StartingNo { get; set; }
        public bool? StartWithYear { get; set; } = false;
        [Required(ErrorMessage = "No Of Digit is Required")]
        [NoOfDigitValidation]
        public int? NoOfDigit { get; set; }
        [Required(ErrorMessage = "Incremental No is Required")]
        [IncrementalNoValidation]
        public int? IncrementalNo { get; set; }
        public bool? TranslationRequired { get; set; } = false;
        public DateTime? LastCreatedDate { get; set; }
        public string? LastNoUsed { get; set; }
        public string? Note { get; set; }
        [Required(ErrorMessage = "Establish No Series Code is Required")]
        [EstablishNoSeriesCodeIDCustomValidation]
        public IEnumerable<long?> EstablishNoSeriesCodeIDs { get; set; } = new List<long?>();
        public bool IsNoSerieswithOrganisationinfo { get; set; } = false;

        public long? CompanyId { get; set; }
        public long? DepartmentId { get; set; }
        [Required(ErrorMessage = "Group is Required")]
        public long? GroupId { get; set; }
        [Required(ErrorMessage = "Group Abbreviation is Required")]
        public string? GroupAbbreviation { get; set; }
        // [Required(ErrorMessage = "Category is Required")]
        public long? CategoryId { get; set; }
        // [Required(ErrorMessage = "Category Abbreviation is Required")]
        public string? CategoryAbbreviation { get; set; }
        public bool? IsGroupAbbreviation { get; set; } = false;
        public bool? IsCategoryAbbreviation { get; set; } = false;
        [Required(ErrorMessage = "Seperator is Required")]
        public int? SeperatorToUse { get; set; }
        public int? LinkId { get; set; }
        [Required(ErrorMessage = "Type of Profile is Required")]
        public int? ProfileTypeId { get; set; }
        public string? SampleDocumentNo { get; set; }
        public bool? IsEnableTask { get; set; } = false;
        public string? DepartmentName { get; set; }
        public string? ProfileType { get; set; }
        public string? GroupName { get; set; }
        public string? CategoryName { get; set; }
        public long? DeparmentId { get; set; }
        public string? IsNoSerieswithOrganisationinfoValue { get; set; }
        public string? AbbreviationRequiredValue { get; set; }
        public string? IsCategoryAbbreviationValue { get; set; }
        public string? IsGroupAbbreviationValue { get; set; }
        public string? StartWithYearValue { get; set; }
        public string? TranslationRequiredValue { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public long? AddedByUserID { get; set; }
        public string? AddedByUser { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string? CompanyName { get; set; }
        public bool IsSampleDocNoEnabled { get; set; } = false;
        public long? FileProfileTypeId { get; set; }
        public long? SelectProfileID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
