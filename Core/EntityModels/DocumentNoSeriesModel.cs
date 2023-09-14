using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentNoSeriesModel
    {
        public long NumberSeriesId { get; set; }
        public long? ProfileID { get; set; }
        public string DocumentNo { get; set; }
        public string VersionNo { get; set; }
        public string Title { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Implementation { get; set; }
        public string ProfileName { get; set; }
        public DateTime? Date { get; set; }
        public long? RequestorId { get; set; }
        public string RequestorName { get; set; }
        public string Link { get; set; }
        public string CompanyCode { get; set; }
        public string DepartmentName { get; set; }
        
        public long? CompanyId { get; set; }
        [Required(ErrorMessage = "Department is Required")]
        public long? DepartmentId { get; set; }
        public DateTime? NextReviewDate { get; set; }
        [Required(ErrorMessage = "Section is Required")]
        public long? SectionId { get; set; }
        [Required(ErrorMessage = "Sub Section is Required")]
        public long? SubSectionId { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? PlantID { get; set; }
        public string PlantCode { get; set; }
        public string ReasonToVoid { get; set; }

        public Guid? SessionID { get; set; }

        public Guid? SessionId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? UploadedByUserID { get; set; }
        public long? DocumentID { get; set; }
        [Required(ErrorMessage = "Division is Required")]
        public long? DivisionId { get; set; }
        //assign to file name
        public string FileName { get; set; }
        public string FileProfileTypeName { get; set; }
        public string Description { get; set; }
        public long? ScreenAutoNumberId { get; set; }
        public List<NumberSeriesCodeModel> Abbreviation1 { get; set; }
        public int? StatusCodeID { get; set; }
        public string StatusCode { get; set; }
        public long? AddedByUserID { get; set; }
        public string AddedByUser { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ScreenID { get; set; }
        public bool? IsUpload { get; set; }
        public string? FilePath { get; set; }

    }
    public class Seperator
    {
        public string SeperatorSymbol { get; set; }
        public int SeperatorValue { get; set; }
    }
    public class MasterListsModel
    {
        public List<Plant> Plants { get; set; }
        public List<Department> Departments { get; set; }
        public List<Section> Sections { get; set; }
        public List<SubSection> SubSections { get; set; }
        public List<DocumentProfileNoSeries> DocumentProfileNoSeries { get; set; }
    }
}
