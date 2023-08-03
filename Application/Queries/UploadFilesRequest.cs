using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace Application.Queries
{
    public class UploadFilesRequest : IRequest<bool>
    {
        public List<IFormFile>? Files { get; set; }
        public Guid SessionId { get; set; }
    }
    public class FileUploadCommand : IRequest<string>
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public Guid SessionId { get; set; }
    }
    public class DownloadFileRequest : IRequest<Documents>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[]? FileData { get; set; }
        public string ContentType { get; set; }
    }
    public class DownloadReportFileRequest : IRequest<ReportDocuments>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
       // public byte[]? FileData { get; set; }
        public string ContentType { get; set; }
    }

}
