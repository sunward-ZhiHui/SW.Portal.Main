
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using System.Net.Mime;

namespace DocumentViewer.Models
{
    public class SpreadsheetDocumentContentFromBytes
    {
        public long? Id { get; set; }
        public string? FileName { get; set; }
        public string DocumentId { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string Extensions { get; set; }
        public string Url { get; set; }
        // public Func<byte[]> ContentAccessorByBytes { get; set; }
        public Func<Stream> ContentAccessorByBytes { get; set; }
        public string? PlainTextBytes { get; set; }
        public bool? IsRead { get; set; } = false;
        public bool? IsDownload { get; set; } = false;
    }
    public class PermissionModel
    {
        public bool? IsRead { get; set; } = false;
        public bool? IsDownload { get; set; } = false;
    }
    public class FileNameModel
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}
