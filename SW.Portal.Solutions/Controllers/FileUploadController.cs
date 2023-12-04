using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using DevExpress.XtraReports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Edm;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Web;
using System.Text.Json;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using AC.SD.Core.Data;
using Microsoft.AspNetCore.SignalR;
using Plugin.Firebase.Firestore;
using System.IO;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        private readonly IReportFileUploadsQueryRepository _fileuploadqueryRepository;
        public FileUploadController(IWebHostEnvironment host, IDocumentsQueryRepository documentsqueryrepository, IReportFileUploadsQueryRepository fileuploadqueryRepository)
        {
            _hostingEnvironment = host;
            _documentsqueryrepository = documentsqueryrepository;
            _fileuploadqueryRepository = fileuploadqueryRepository;
        }
        [HttpPost]
        [Route("UploadDocumentsBySession")]
        public async Task<ActionResult> UploadDocumentsBySession(IFormFile files, Guid? SessionId, long? addedByUserId, bool? IsFileSession, string? SourceFrom)
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
                    //RemoveTempFilesAfterDelay(serverPaths, new TimeSpan(0, 5, 0));

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
                        documents.SessionId = IsFileSession == true ? metaDataObject.FileGuid : SessionId;
                        documents.IsLatest = true;
                        documents.IsTemp = true;
                        documents.FileName = metaDataObject.FileName;
                        documents.ContentType = metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.SourceFrom = SourceFrom;
                        documents.FilePath = serverPath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                        var response = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                        documentId = response.DocumentId;
                        System.GC.Collect();
                        GC.SuppressFinalize(this);
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
        public async Task<ActionResult> UploadDocumentsById(IFormFile files, Guid? SessionId, long? DocumentId, string? SourceFrom)
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
                    // RemoveTempFilesAfterDelay(serverPaths, new TimeSpan(0, 5, 0));

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
                        documents.SourceFrom = SourceFrom;
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


        [HttpPost]
        [Route("UploadReportFile")]
        public async Task<ActionResult> UploadReportFile(IFormFile files, Guid? SessionId, long? addedByUserId)
        {
            long ReportdocumentId = 0;
            // Handling Upload with Chunks
            string chunkMetadata = Request.Form["chunkMetadata"];

            // Set BasePath
            string BaseDirectory = System.AppContext.BaseDirectory;
            //  var reportFolder = Path.Combine(BaseDirectory, "Reports");
            var serverPaths = Path.Combine(BaseDirectory, "Reports");

            try
            {
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonSerializer.Deserialize<ChunkMetadata>(chunkMetadata);

                    // Use tmp File for Upload
                    var tempFilePath = Path.Combine(serverPaths, metaDataObject.FileName + ".tmp");

                    // Create UploadFolder
                    if (!System.IO.Directory.Exists(serverPaths))
                    {
                        System.IO.Directory.CreateDirectory(serverPaths);
                    }
                    // Removes temporary files 
                    //RemoveTempFilesAfterDelay(serverPaths, new TimeSpan(0, 5, 0));

                    // Save FileStream
                    using (var stream = new FileStream(tempFilePath, FileMode.Append, FileAccess.Write))
                    {
                        files.CopyTo(stream);
                    }
                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                    {
                        var ext = metaDataObject.FileName.Split(".").Last();
                        var fileName1 = metaDataObject.FileName;
                        var FileName = Path.GetFileNameWithoutExtension(metaDataObject.FileName);
                        var serverPath = serverPaths + @"\" + fileName1;

                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, serverPath), true);
                        System.IO.File.Delete(tempFilePath);
                        ReportDocuments documents = new ReportDocuments();


                        documents.IsLatest = true;
                        //documents.IsTemp = true;
                        documents.FileName = FileName;
                        documents.ContentType = metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.SessionId = SessionId;
                        documents.FilePath = serverPath.Replace(Path.Combine(BaseDirectory, "Reports"), "");
                        var response = await _fileuploadqueryRepository.InsertCreateReportDocumentBySession(documents);
                        ReportdocumentId = response.ReportDocumentID;
                        System.GC.Collect();
                        GC.SuppressFinalize(this);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(ReportdocumentId.ToString());
        }
        [HttpPost]
        [Route("UploadImages")]
        public async Task<ActionResult> UploadImages(IFormCollection files)
        {
            try
            {
                var sessionID = new Guid(files["UploadSessionId"].ToString());
                var addedByUserId = Convert.ToInt32(files["UserID"].ToString());
                var SourceFrom = files["SourceFrom"].ToString();
                var ChangeNewFileName = files["ChangeNewFileName"].ToString();
                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + sessionID + @"\";
                string fileName = sessionID + ".pdf";
                var serverFilePath = serverPaths + fileName;
                if (!System.IO.Directory.Exists(serverPaths))
                {
                    System.IO.Directory.CreateDirectory(serverPaths);
                }
                using (var memoryStream = new MemoryStream())
                {

                    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(memoryStream));
                    Document document = new Document(pdfDocument);
                    int i = 0;
                    foreach (var f in files.Files)
                    {
                        var file = f;
                        var fs = file.OpenReadStream();
                        var br = new BinaryReader(fs);
                        Byte[] documentByte = br.ReadBytes((Int32)fs.Length);
                        var image = new Image(ImageDataFactory.Create(documentByte));
                        pdfDocument.AddNewPage(new iText.Kernel.Geom.PageSize(image.GetImageWidth(), image.GetImageHeight()));
                        image.SetFixedPosition(i + 1, 0, 0);
                        document.Add(image);
                        i++;
                    }
                    pdfDocument.Close();
                    byte[] fileData = memoryStream.ToArray();
                    var fileSize = fileData.Length;
                    await System.IO.File.WriteAllBytesAsync(serverFilePath, fileData);
                    Documents documents = new Documents();
                    documents.UploadDate = DateTime.Now;
                    documents.AddedByUserId = addedByUserId;
                    documents.AddedDate = DateTime.Now;
                    documents.SessionId = sessionID;
                    documents.IsLatest = true;
                    documents.IsTemp = true;
                    documents.FileName = ChangeNewFileName + ".pdf";
                    documents.ContentType = "application/pdf";
                    documents.FileSize = fileSize;
                    documents.SourceFrom = SourceFrom;
                    documents.FilePath = serverFilePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                    var response = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                    System.GC.Collect();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok("Ok");
        }
    }
}
