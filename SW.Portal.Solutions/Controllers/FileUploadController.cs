using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentsQueryRepository _documentsqueryrepository;

        public FileUploadController(IWebHostEnvironment host, IDocumentsQueryRepository documentsqueryrepository)
        {
            _hostingEnvironment = host;
            _documentsqueryrepository = documentsqueryrepository;
        }
        [HttpPost]
        [Route("UploadDocumentsBySession")]
        public async Task<ActionResult> UploadDocumentsBySession(IFormFile files, Guid? SessionId, long? addedByUserId)
        {
            long documentId = 0;
            // Handling Upload with Chunks
            string chunkMetadata = Request.Form["chunkMetadata"];

            // Set BasePath
            var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;

            try
            {
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonSerializer.Deserialize<ChunkMetadata>(chunkMetadata);

                    // Use tmp File for Upload
                    var tempFilePath = Path.Combine(serverPaths, metaDataObject.FileGuid + ".tmp");

                    // Create UploadFolder
                    if (!System.IO.Directory.Exists(serverPaths))
                    {
                        System.IO.Directory.CreateDirectory(serverPaths);
                    }

                    // Removes temporary files 
                    RemoveTempFilesAfterDelay(serverPaths, new TimeSpan(0, 5, 0));

                    // Save FileStream
                    using (var stream = new FileStream(tempFilePath, FileMode.Append, FileAccess.Write))
                    {
                        files.CopyTo(stream);
                    }
                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        var ext = metaDataObject.FileName.Split(".").Last();
                        var fileName1 = metaDataObject.FileGuid + "." + ext;
                        var serverPath = serverPaths + @"\" + fileName1;

                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, serverPath), true);
                        System.IO.File.Delete(tempFilePath);
                        Documents documents = new Documents();                       
                        documents.UploadDate = DateTime.Now;
                        documents.AddedByUserId = addedByUserId;
                        documents.AddedDate = DateTime.Now;
                        documents.SessionId = SessionId;
                        documents.IsLatest = true;
                        documents.FileName = metaDataObject.FileName;
                        documents.ContentType = metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.FilePath = serverPath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                        var response = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                        documentId = response.DocumentId;
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(documentId.ToString());
        }
        [HttpPost]
        [Route("UploadDocumentsById")]
        public async Task<ActionResult> UploadDocumentsById(IFormFile files, Guid? SessionId, long? DocumentId)
        {
            // Handling Upload with Chunks
            string chunkMetadata = Request.Form["chunkMetadata"];

            // Set BasePath
            var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;

            try
            {
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonSerializer.Deserialize<ChunkMetadata>(chunkMetadata);

                    // Use tmp File for Upload
                    var tempFilePath = Path.Combine(serverPaths, metaDataObject.FileGuid + ".tmp");

                    // Create UploadFolder
                    if (!System.IO.Directory.Exists(serverPaths))
                    {
                        System.IO.Directory.CreateDirectory(serverPaths);
                    }

                    // Removes temporary files 
                    RemoveTempFilesAfterDelay(serverPaths, new TimeSpan(0, 5, 0));

                    // Save FileStream
                    using (var stream = new FileStream(tempFilePath, FileMode.Append, FileAccess.Write))
                    {
                        files.CopyTo(stream);
                    }
                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        var ext = metaDataObject.FileName.Split(".").Last();
                        var fileName1 = metaDataObject.FileGuid + "." + ext;
                        var serverPath = serverPaths + @"\" + fileName1;

                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, serverPath), true);
                        System.IO.File.Delete(tempFilePath);
                        Documents documents = new Documents();
                        documents.DocumentId = DocumentId.Value;
                        documents.FileName = metaDataObject.FileName;
                        documents.ContentType = metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.FilePath = serverPath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                        await _documentsqueryrepository.UpdateDocumentAfterUpload(documents);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
        void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                    file.Delete();
            }
        }
    }
}
