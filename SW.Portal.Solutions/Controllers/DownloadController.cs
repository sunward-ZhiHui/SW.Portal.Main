using Application.Queries;
using Core.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _DocumentPath;

        public DownloadController(IMediator mediator, IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpGet("DownloadAllFileProfileTypeItem")]
        public async Task<ActionResult> DownloadAllFileProfileTypeItem(int FileProfileTypeId, Guid SessionId)
        {
            DocumentSearchModel documentSearchModel = new() { SessionId = SessionId, FileProfileTypeId = FileProfileTypeId };

            var _fileProfilelstItems = await _mediator.Send(new GetAllSelectedfileprofiletypeQuery(documentSearchModel));
            var fileProfileTypeDocuments = _fileProfilelstItems.DocumentsData.Where(x=>x.Type == "Document");

            // Stream zip directly to response
            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var Document in fileProfileTypeDocuments)
                {
                    string filePath = string.Empty;
                    if (Document.IsNewPath == true)
                    {
                        filePath = Path.Combine(_configuration["DocumentsUrl:NewFileLivePath"], Document.FilePath);

                    }
                    else
                    {
                        filePath = Path.Combine(_configuration["DocumentsUrl:OldFileLivePath"], Document.FilePath);
                        //filePath = Path.Combine(_webHostEnvironment.ContentRootPath, Document.FilePath);


                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        var entryName = Document.FileName; // flatten folder structure
                        var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);

                        using var entryStream = entry.Open();
                        using var fileStream = System.IO.File.OpenRead(filePath);
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }

            zipStream.Position = 0; // reset stream before returning

            return File(zipStream, "application/zip", "AllFiles.zip");
        }
    }
}
