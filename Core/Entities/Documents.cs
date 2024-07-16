using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Documents
    {
        [Key]
        public long DocumentId { get; set; }
        public string? FileName { get; set; }
        public string? DisplayName { get; set; }
        public string? Extension { get; set; }
        public string? ContentType { get; set; }
        public int? DocumentType { get; set; }
        public byte[]? FileData { get; set; }
        public long? FileSize { get; set; }
        public DateTime? UploadDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? LinkId { get; set; }
        public bool? IsSpecialFile { get; set; }
        public bool? IsTemp { get; set; }
        public long? DepartmentId { get; set; }
        public long? WikiId { get; set; }
        public long? CategoryId { get; set; }
        public int? StatusCodeId { get; set; }
        public int? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? FilterProfileTypeId { get; set; }
        public string? ProfileNo { get; set; }
        public string? TableName { get; set; }
        public long? DocumentParentId { get; set; }
        public string? ScreenId { get; set; }
        public bool? IsLatest { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? LockedDate { get; set; }
        public long? LockedByUserId { get; set; }
        public bool? IsWikiDraft { get; set; }
        public bool? IsWikiDraftDelete { get; set; }
        public bool? IsMobileUpload { get; set; }
        public bool? IsCompressed { get; set; }
        public bool? IsHeaderImage { get; set; }
        public int? FileIndex { get; set; }
        public bool? IsVideoFile { get; set; }
        public int? CloseDocumentId { get; set; }
        public int? ArchiveStatusId { get; set; }
        public bool? IsPublichFolder { get; set; }
        public long? FolderId { get; set; }
        public long? TaskId { get; set; }
        public bool? IsMainTask { get; set; }
        public bool? IsPrint { get; set; }
        public bool? IsWiki { get; set; }
        public string? SubjectName { get; set; }
        public bool? IsNewPath { get; set; }
        public long? DeleteByUserID { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteByDate { get; set; }
        public string? FilePath { get; set; }
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? ServerFilePath { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? SourceFrom { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public long? EmailToDMS { get; set; }
        [NotMapped]
        public string DMSBackUrl { get; set; }
        [NotMapped]
        public long ReplaceDocumentId { get; set; }
        public long? SwProfileTypeId { get; set; }
    }
}
