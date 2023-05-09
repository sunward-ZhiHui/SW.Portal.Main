using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewEmployee
    {
        public long EmployeeID { get; set; }
        public long? UserID { get; set; }
        public string SageID { get; set; }
        public long? PlantID { get; set; }
        public long? LevelID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string UserCode { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public int? TypeOfEmployeement { get; set; }
        public long? LanguageID { get; set; }

        public long? CityID { get; set; }
        public long? RegionID { get; set; }
        public string Signature { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? DateOfEmployeement { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string Extension { get; set; }
        public string SpeedDial { get; set; }
        public string Mobile { get; set; }
        public string SkypeAddress { get; set; }
        public long? ReportID { get; set; }
        public bool? IsActive { get; set; }
        public long? DivisionID { get; set; }
        public string LoginID { get; set; }
        public string LoginPassword { get; set; }
        public long? SectionID { get; set; }
        public long? SubSectionID { get; set; }
        public long? SubSectionTID { get; set; }
        public long? DesignationID { get; set; }
        public long? DepartmentID { get; set; }
        public long? AcceptanceStatus { get; set; }
        public string Status { get; set; }
        public DateTime? AcceptanceStatusDate { get; set; }
        public DateTime? ExpectedJoiningDate { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string SubSectionTwoName { get; set; }
        public string DivisionName { get; set; }
        public int? StatusCodeID { get; set; }
        public string StatusCode { get; set; }
        public long? AddedByUserID { get; set; }
        public string AddedByUser { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CompanyName { get; set; }
        public int? HeadCount { get; set; }
        public Guid? SessionId { get; set; }
    }
}
