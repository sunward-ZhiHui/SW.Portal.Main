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
        public ActionResult UploadDocuments(IFormFile files, Guid? SessionId)
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
                    AddedByUserId = null,
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
        [Route("DownloadFile")]
        public async Task<ActionResult> DownloadFile(long DocumentId)
        {
            var lst = _context.Documents.Where(x=>x.DocumentId == DocumentId).ToList();
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
    }
}
