using Application.Common;
using Application.Queries;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{

    public class EmployeeResponse
    {
        public long EmployeeID { get; set; }

        public long? UserID { get; set; }
        [Required(ErrorMessage = "Sage ID is Required")]
        public string SageID { get; set; }
        public long? PlantID { get; set; }
        public long? LevelID { get; set; }
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string UserCode { get; set; }
        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Employeement Type is Required")]

        public int? TypeOfEmployeement { get; set; }
        [Required(ErrorMessage = "Language is Required")]
        public long? LanguageID { get; set; }
        [Required(ErrorMessage = "User Role is Required")]
        public long? RoleID { get; set; }
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
        [Required(ErrorMessage = "LoginID is Required")]
        public string LoginID { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string LoginPassword { get; set; }
        public long? SectionID { get; set; }
        public long? SubSectionID { get; set; }
        public long? SubSectionTID { get; set; }
        [Required(ErrorMessage = "Designation is Required")]
        public long? DesignationID { get; set; }
        public long? DepartmentID { get; set; }
        [Required(ErrorMessage = "Employement Status is Required")]
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
        public IEnumerable<long?> ReportToIds { get; set; }
        public Guid? SessionId { get; set; }
    }
}
