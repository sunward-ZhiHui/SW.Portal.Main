namespace DocumentViewer.Models
{
    public class Employee
    {
        public long EmployeeId { get; set; }
        public long? UserId { get; set; }
        public string? SageId { get; set; }
        public long? PlantId { get; set; }
        public long? LevelId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public string? FirstName { get; set; }
        public string? NickName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public int? TypeOfEmployeement { get; set; }
        public long? LanguageId { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        public long? CityId { get; set; }
        public long? RegionId { get; set; }
        public string? Signature { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateOfEmployeement { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string? Extension { get; set; }
        public string? SpeedDial { get; set; }
        public string? Mobile { get; set; }
        public string? SkypeAddress { get; set; }
        public long? ReportId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public long? SubSectionTid { get; set; }
        public int? HeadCount { get; set; }
        public long? DivisionId { get; set; }
        public byte[]? AcceptanceLetter { get; set; }
        public DateTime? ExpectedJoiningDate { get; set; }
        public long? AcceptanceStatus { get; set; }
        public DateTime? AcceptanceStatusDate { get; set; }
        public string? Nricno { get; set; }
    }
}
