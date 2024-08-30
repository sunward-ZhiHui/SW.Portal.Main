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
using SW.Portal.Solutions.Models;
using AC.SD.Core.Pages.Masters;
using Grpc.Core;
using AC.SD.Core.Pages.DMS;
using Application.Queries;
using MediatR;
using Google.Cloud.Firestore;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        private readonly IReportFileUploadsQueryRepository _fileuploadqueryRepository;
        private readonly IMediator _mediator;
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public FileUploadController(IWebHostEnvironment host, IDocumentsQueryRepository documentsqueryrepository, IReportFileUploadsQueryRepository fileuploadqueryRepository, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository, IMediator mediator)
        {
            _hostingEnvironment = host;
            _documentsqueryrepository = documentsqueryrepository;
            _fileuploadqueryRepository = fileuploadqueryRepository;
            _mediator = mediator;
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        [HttpPost]
        [Route("UploadDocumentsBySession")]
        public async Task<ActionResult> UploadDocumentsBySession(IFormFile files, Guid? SessionId, long? addedByUserId, bool? IsFileSession, string? SourceFrom, long? FileProfileId)
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
                        documents.ContentType = ext.ToLower() == "msg" ? "application/octet-stream" : metaDataObject.FileType;
                        documents.FileSize = metaDataObject.FileSize;
                        documents.SourceFrom = SourceFrom;
                        documents.SwProfileTypeId = FileProfileId;
                        documents.FilePath = serverPath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                        var response = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                        documentId = response.DocumentId;
                        //files = null;
                        //chunkMetadata = null;
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
                        var filePath = Path.Combine(serverPaths, file.FileName);
                        Byte[] documentByte = br.ReadBytes((Int32)fs.Length);
                        var image = new Image(ImageDataFactory.Create(documentByte));
                        pdfDocument.AddNewPage(new iText.Kernel.Geom.PageSize(image.GetImageWidth(), image.GetImageHeight()));
                        image.SetFixedPosition(i + 1, 0, 0);
                        document.Add(image);
                        i++;

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(fileStream);
                        //}
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
        [HttpPost("MobileUploadFile")]
        public async Task<ResponseModel> MobileUploadFile([FromForm] Guid? SessionId, [FromForm] long? addedByUserId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid content type.";
                    return response;
                }

                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No file uploaded.";
                    return response;
                }

                var serverPaths = Path.Combine(_hostingEnvironment.ContentRootPath, "AppUpload", "Documents", SessionId.ToString());
                if (!Directory.Exists(serverPaths))
                {
                    Directory.CreateDirectory(serverPaths);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Appending the extension to the filename
                var filePath = Path.Combine(serverPaths, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var contentType = file.ContentType;
                var fileSize = file.Length;
                var fileExtension = Path.GetExtension(file.FileName); // Extracting the file extension

                Documents documents = new Documents();
                documents.UploadDate = DateTime.Now;
                documents.AddedByUserId = addedByUserId;
                documents.AddedDate = DateTime.Now;
                documents.SessionId = SessionId;
                documents.IsLatest = true;
                documents.IsTemp = true;
                documents.FileName = fileName;
                documents.ContentType = contentType;
                documents.FileSize = fileSize;
                documents.SourceFrom = "FileProfile";
                documents.FilePath = serverPaths.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                var responsesss = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);

                response.IsSuccess = true;
                response.Message = $"File uploaded successfully. Content Type: {contentType}, File size: {fileSize} bytes, File extension: {fileExtension}";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Internal server error: {ex.Message}";
                return response;
            }
        }
        [HttpPost("MobileFileProfileType")]

        public async Task<ResponseModel> MobileFileProfileType([FromForm] Models.FileProfileTypeModel value)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid content type.";
                    return response;
                }

                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No file uploaded.";
                    return response;
                }
                //var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;

                // var serverPaths = Path.Combine(_hostingEnvironment.ContentRootPath, "AppUpload", "Documents", value.SessionId.ToString());

                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + value.SessionId.ToString();
                if (!Directory.Exists(serverPaths))
                {
                    Directory.CreateDirectory(serverPaths);
                }

                var FileProfileSessionID = await _mediator.Send(new GetFileProfileTypeList(value.FileProfileTypeId));
                var FileSessionID = FileProfileSessionID.SessionID;
                Guid? FileNameSessionID = Guid.NewGuid();
                if (FileSessionID != null)
                {
                    FileNameSessionID = FileSessionID;
                }
                var fileName = FileNameSessionID.ToString() + Path.GetExtension(file.FileName); // Appending the extension to the filename
                var filePath = Path.Combine(serverPaths, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var contentType = file.ContentType;
                var fileSize = file.Length;
                var fileExtension = Path.GetExtension(file.FileName); // Extracting the file extension




                // var serverPath = Path.Combine(_hostingEnvironment.ContentRootPath, "AppUpload", "Documents", value.SessionId.ToString(), @"\", FileSessionID, ".", fileExtension);
                var serverPath = serverPaths + @"\" + FileSessionID.ToString() + fileExtension;

                //  var serverPath = Path.Combine(_hostingEnvironment.ContentRootPath, "AppUpload", "Documents", value.SessionId.ToString(), FileSessionID.ToString(), fileExtension);
                var documentNoSeriesModel = new DocumentNoSeriesModel
                {
                    AddedByUserID = value.UserID,
                    StatusCodeID = 710,
                    ProfileID = value.ProfileId,
                    PlantID = value.PlantId,
                    DepartmentId = value.DepartmentId,
                    SectionId = value.SectionId,
                    SubSectionId = value.SubSectionId,
                    DivisionId = value.DivisionId,

                };
                var profileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
                Documents documents = new Documents();
                documents.UploadDate = DateTime.Now;
                documents.AddedByUserId = value.addedByUserId;
                documents.AddedDate = DateTime.Now;
                documents.SessionId = value.SessionId;
                documents.IsLatest = true;
                documents.IsTemp = true;
                documents.FileName = file.FileName;
                documents.ContentType = contentType;
                documents.FileSize = fileSize;
                documents.FilterProfileTypeId = value.FileProfileTypeId;
                documents.SourceFrom = "FileProfile";
                documents.ProfileNo = profileNo;
                documents.FilePath = serverPath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                var responsesss = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);

                response.IsSuccess = true;
                response.Message = $"File uploaded successfully. Content Type: {contentType}, File size: {fileSize} bytes, File extension: {fileExtension}";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Internal server error: {ex.Message}";
                return response;
            }
        }
        [HttpPost]
        [Route("UploadDynamicFormReportFile")]
        public async Task<ActionResult> UploadDynamicFormReportFile(IFormFile files, Guid? SessionId, long? addedByUserId)
        {
            long ReportdocumentId = 0;
            // Handling Upload with Chunks
            string chunkMetadata = Request.Form["chunkMetadata"];

            // Set BasePath
            string BaseDirectory = System.AppContext.BaseDirectory;
            var serverPaths = Path.Combine(BaseDirectory, "Reports");

            try
            {
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    var metaDataObject = JsonSerializer.Deserialize<ChunkMetadata>(chunkMetadata);

                    // Use tmp File for Upload
                    var tempFilePath = Path.Combine(serverPaths, SessionId + ".tmp");

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
                        var fileName1 = SessionId + "." + ext;
                        var serverPath = serverPaths + @"\" + fileName1;

                        var FileName = Path.GetFileNameWithoutExtension(metaDataObject.FileName);

                        // Upload finished - overwrite/copy file and remove tempFile
                        System.IO.File.Copy(tempFilePath, Path.Combine(serverPaths, serverPath), true);
                        System.IO.File.Delete(tempFilePath);
                        DynamicFormReport documents = new DynamicFormReport();

                        documents.FileName = FileName;
                        documents.ContentType = files.ContentType;
                        documents.FileSize = files.Length;
                        documents.SessionId = SessionId;
                        documents.FilePath = serverPath.Replace(Path.Combine(BaseDirectory, "Reports"), "");
                        var response = await _fileuploadqueryRepository.UpdateDynamicFormReportBySession(documents);
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


        [HttpPost("MobileFileProfileTypeMultiple")]

        public async Task<ResponseModel> MobileFileProfileTypeMultiple([FromForm] Models.FileProfileTypeModel value)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid content type.";
                    return response;
                }

                var file = Request.Form.Files;
                if (file == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No file uploaded.";
                    return response;
                }


                var sessionID = value.SessionId;
                var addedByUserId = value.UserID;
                var SourceFrom = "FileProfile";
                var ChangeNewFileName = value.NewFilename;
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
                    foreach (var f in file)
                    {
                        var files = f;
                        var fs = files.OpenReadStream();
                        var br = new BinaryReader(fs);
                        var filePath = Path.Combine(serverPaths, files.FileName);
                        Byte[] documentByte = br.ReadBytes((Int32)fs.Length);
                        var image = new Image(ImageDataFactory.Create(documentByte));
                        pdfDocument.AddNewPage(new iText.Kernel.Geom.PageSize(image.GetImageWidth(), image.GetImageHeight()));
                        image.SetFixedPosition(i + 1, 0, 0);
                        document.Add(image);
                        i++;

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await file.CopyToAsync(fileStream);
                        //}
                    }
                    pdfDocument.Close();
                    byte[] fileData = memoryStream.ToArray();
                    var fileSize = fileData.Length;
                    await System.IO.File.WriteAllBytesAsync(serverFilePath, fileData);
                    var documentNoSeriesModel = new DocumentNoSeriesModel
                    {
                        AddedByUserID = value.UserID,
                        StatusCodeID = 710,
                        ProfileID = value.ProfileId,
                        PlantID = value.PlantId,
                        DepartmentId = value.DepartmentId,
                        SectionId = value.SectionId,
                        SubSectionId = value.SubSectionId,
                        DivisionId = value.DivisionId,

                    };
                    var profileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
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
                    documents.ProfileNo = profileNo;
                    documents.FilePath = serverFilePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                    var responses = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                    System.GC.Collect();
                    GC.SuppressFinalize(this);
                    response.IsSuccess = true;
                    //response.Message = $"File uploaded successfully. Content Type: {contentType}, File size: {fileSize} bytes, File extension: {fileExtension}";
                    return response;
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Internal server error: {ex.Message}";
                return response;
            }
        }
    }
}
