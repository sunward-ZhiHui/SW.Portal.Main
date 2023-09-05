using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentNoSeriesModel : BaseModel
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
        public long? DepartmentId { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public long? PlantID { get; set; }
        public string PlantCode { get; set; }
        public string ReasonToVoid { get; set; }

        public Guid? SessionID { get; set; }

        public Guid? SessionId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? UploadedByUserID { get; set; }
        public long? DocumentID { get; set; }
        public long? DivisionId { get; set; }
        //assign to file name
        public string FileName { get; set; }
        public string Description { get; set; }
        public long? ScreenAutoNumberId { get; set; }
        public List<NumberSeriesCodeModel> Abbreviation1 { get; set; }



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
