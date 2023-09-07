using Azure.Messaging;
using DocumentApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DocumentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public UploadController(AppDbContext context, IWebHostEnvironment host)
        {
            _hostingEnvironment = host;
            _context = context;
        }
        [HttpPost]
        [Route("UploadDocuments")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public ActionResult UploadDocuments(IFormFile files, Guid? SessionId, long? addedByUserId)
        {
            long documentId = 0;
            try
            {
                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;
                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }
                UpdateIsLatest(SessionId);
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
                var documents = new Documents
                {
                    FileName = files.FileName,
                    ContentType = files.ContentType,
                    FileData = null,
                    FileSize = files.Length,
                    UploadDate = DateTime.Now,
                    AddedByUserId = addedByUserId,
                    AddedDate = DateTime.Now,
                    SessionId = SessionId,
                    IsLatest = true,
                    FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", ""),

                };
                _context.Documents.Add(documents);
                _context.SaveChanges();
                documentId = documents.DocumentId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message", ex);
            }
            return Ok(documentId.ToString());
        }
        [HttpPost]
        [Route("UploadDocumentsByID")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public ActionResult UploadDocumentsByID(IFormFile files, Guid? SessionId, long? DocumentId)
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
                var documents = _context.Documents.SingleOrDefault(s => s.DocumentId == DocumentId);
                if (documents != null)
                {
                    documents.FileName = files.FileName;
                    documents.ContentType = files.ContentType;
                    documents.FileSize = files.Length;
                    documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                    _context.SaveChanges();
                }
                documentId = DocumentId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Message", ex);
            }
            return Ok(documentId.ToString());
        }
        [HttpPost]
        [Route("DownloadFile")]
        public async Task<ActionResult> DownloadFile(long DocumentId)
        {
            var lst = _context.Documents.Where(x => x.DocumentId == DocumentId).ToList();
            if (lst.Count > 0)
            {
                var BaseUrl = _hostingEnvironment.ContentRootPath + @"\AppUpload\";

                var filePath = BaseUrl + lst[0].FilePath;
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", lst[0].FileName);
                }
            }
            return NotFound();
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
        private void UpdateIsLatest(Guid? SessionId)
        {
            var query = string.Format("Update Documents Set IsLatest='{1}' Where SessionId='{0}'", SessionId, 0);
            _context.Database.ExecuteSqlRaw(query);
        }

        [HttpGet]
        [Route("GetString")]
        public string GetString()
        {
            return "OK";
        }
        [HttpPost]
        [Route("uploadchunkfileById")]
        public ActionResult uploadchunkfileById(IFormFile files, Guid? SessionId, long? DocumentId)
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
                        var documents = _context.Documents.SingleOrDefault(s => s.DocumentId == DocumentId);
                        if (documents != null)
                        {
                            documents.FileName = metaDataObject.FileName;
                            documents.ContentType = metaDataObject.FileType;
                            documents.FileSize = metaDataObject.FileSize;
                            documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                            _context.SaveChanges();
                        }
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
