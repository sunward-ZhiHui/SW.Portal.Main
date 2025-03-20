using AC.SD.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string localAttachmentPath = "C:\\EmailAttachments\\";
        private readonly AttachmentService _attachmentService;

        public FileController(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }


        [HttpGet("download1")]
        public IActionResult DownloadFile1([FromQuery] string filename)
        {
            string filePath = Path.Combine(localAttachmentPath, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", filename);
        }

        [HttpGet("download/{token}/{filename}")]
        public IActionResult DownloadFile(string token, string filename)
        {
            var fileData = _attachmentService.GetAttachment(token);
            if (fileData == null)
            {
                return NotFound("Attachment not found.");
            }

            // Remove attachment from memory after download (optional)
            _attachmentService.RemoveAttachment(token);

            return File(fileData.Value.Data, fileData.Value.ContentType, filename);
        }
    }
}
