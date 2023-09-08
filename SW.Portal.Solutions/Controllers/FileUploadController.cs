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
        [Route("UploadDocuments")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<ActionResult> UploadDocuments(IFormFile files, Guid? SessionId, long? addedByUserId)
        {
            long documentId = 0;
            try
            {
                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;
                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }
                var ext = "";
                var newFile = "";
                ext = files.FileName;
                int i = ext.LastIndexOf('.');
                string lhs = i < 0 ? ext : ext.Substring(0, i), rhs = i < 0 ? "" : ext.Substring(i + 1);
                var fileName1 = Guid.NewGuid() + "." + rhs;
                var serverPath = serverPaths + @"\" + fileName1;
                var filePath = getNextFileName(serverPath);
                newFile = filePath.Replace(serverPaths + @"\", "");
                using (var targetStream = System.IO.File.Create(serverPath))
                {
                    files.CopyTo(targetStream);
                    targetStream.Flush();
                }
                Documents documents = new Documents();
                documents.FileName = files.FileName;
                documents.ContentType = files.ContentType;
                documents.FileSize = files.Length;
                documents.UploadDate = DateTime.Now;
                documents.AddedByUserId = addedByUserId;
                documents.AddedDate = DateTime.Now;
                documents.SessionId = SessionId;
                documents.IsLatest = true;
                documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                var response=await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                documentId = response.DocumentId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message", ex);
            }
            return Ok(documentId.ToString());
        }
        [HttpPost]
        [Route("UploadDocumentsChunkFile")]
        public async Task<ActionResult> UploadDocumentsChunkFile(IFormFile files, Guid? SessionId, long? addedByUserId)
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
                        var fileName1 = Guid.NewGuid() + "." + ext;
                        var serverPath = serverPaths + @"\" + fileName1;
                        var filePath = getNextFileName(serverPath);


                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, filePath), true);
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
                        documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
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
        [Route("UploadDocumentsChunkFileById")]
        public async Task<ActionResult> UploadchunkfileById(IFormFile files, Guid? SessionId, long? DocumentId)
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
                        var fileName1 = Guid.NewGuid() + "." + ext;
                        var serverPath = serverPaths + @"\" + fileName1;
                        var filePath = getNextFileName(serverPath);


                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, filePath), true);
                        System.IO.File.Delete(tempFilePath);
                        Documents documents = new Documents();
                        documents.DocumentId = DocumentId.Value;
                        documents.FileName = metaDataObject.FileName;
                        documents.ContentType = metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
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
        [HttpPost]
        [Route("UploadDocumentsById")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<ActionResult> UploadDocumentsByID(IFormFile files, Guid? SessionId, long? DocumentId)
        {
            long? documentId = 0;
            try
            {
                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;
                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }
                var ext = "";
                var newFile = "";
                ext = files.FileName;
                int i = ext.LastIndexOf('.');
                string lhs = i < 0 ? ext : ext.Substring(0, i), rhs = i < 0 ? "" : ext.Substring(i + 1);
                var fileName1 = Guid.NewGuid() + "." + rhs;
                var serverPath = serverPaths + @"\" + fileName1;
                var filePath = getNextFileName(serverPath);
                newFile = filePath.Replace(serverPaths + @"\", "");
                using (var targetStream = System.IO.File.Create(serverPath))
                {
                    files.CopyTo(targetStream);
                    targetStream.Flush();
                }
                Documents documents = new Documents();
                documents.DocumentId = DocumentId.Value;
                documents.FileName = files.FileName;
                documents.ContentType = files.ContentType;
                documents.FileSize = files.Length;
                documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                await _documentsqueryrepository.UpdateDocumentAfterUpload(documents);
                documentId = DocumentId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message", ex);
            }
            return Ok(DocumentId.ToString());
        }
        private string getNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            int i = 0;
            while (System.IO.File.Exists(fileName))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }

            return fileName;
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
