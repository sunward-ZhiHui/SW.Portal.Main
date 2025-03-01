namespace DocumentViewer.Models
{
    public class FileProfileType
    {
        public long FileProfileTypeId { get; set; }
        public long ProfileId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? ParentId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsExpiryDate { get; set; }
        public bool? IsAllowMobileUpload { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public int? ShelfLifeDuration { get; set; }
        public int? ShelfLifeDurationId { get; set; }
        public string? Hints { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public string? ProfileTypeInfo { get; set; }
        public bool? IsCreateByYear { get; set; }
        public bool? IsCreateByMonth { get; set; }
        public bool? IsHidden { get; set; }
        public string? ProfileInfo { get; set; }
        public bool? IsTemplateCaseNo { get; set; }
        public long? TemplateTestCaseId { get; set; }
        public Guid? SessionId { get; set; }
        public bool? IsDelete { get; set; }
        public long? DeleteByUserId { get; set; }
        public DateTime? DeleteByDate { get; set; }
        public long? RestoreByUserId { get; set; }
        public DateTime? RestoreByDate { get; set; }
        public long? DynamicFormId { get; set; }
        public bool? IsDuplicateUpload { get; set; }
    }
}
