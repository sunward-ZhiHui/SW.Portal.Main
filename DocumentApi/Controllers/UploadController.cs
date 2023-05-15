using DocumentApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace DocumentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult UploadDocuments(IFormFile files)
        {
            try
            {
               /* var SessionId = new Guid(files["sessionId"].ToString());
                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Videos\" + SessionId; 
                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }

                using (var targetStream = System.IO.File.Create(serverPath))
                {
                    files.CopyTo(targetStream);
                    targetStream.Flush();
                }*/
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet]
        [Route("Get")]
        public Documents? Get(long id)
        {
            var documents = _context.Documents.FirstOrDefault(a => a.DocumentId == id);
            return documents;
        }
    }
}
