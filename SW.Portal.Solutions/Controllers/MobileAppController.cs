using AC.SD.Core.Data;
using Application.Queries;
using Application.Queries.Base;
using ChartJs.Blazor.ChartJS.Common.Axes;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Infrastructure.Repository.Query;
using iText.IO.Image;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;
using System.Text.Json;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Application.Response;
using Core.Entities.Views;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAppController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMediator _mediator;
        private readonly IFbOutputCartonsQueryRepository _FbOutputCartonsQueryRepository;
        private readonly IQCTimesheetQueryRepository _qcTimeSheetQueryRepository;
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public MobileAppController(IDocumentsQueryRepository documentsqueryrepository, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository,IWebHostEnvironment hostingEnvironment, IMediator mediator, IFbOutputCartonsQueryRepository FbOutputCartonsQueryRepository, IQCTimesheetQueryRepository qcTimeSheetQueryRepository)
        {
            _mediator = mediator;
            _FbOutputCartonsQueryRepository = FbOutputCartonsQueryRepository;
            _qcTimeSheetQueryRepository = qcTimeSheetQueryRepository;
            _hostingEnvironment = hostingEnvironment;
            _documentsqueryrepository = documentsqueryrepository;
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        [HttpGet("GetFbOutputCartonsList")]
        public async Task<ActionResult<Services.ResponseModel<List<FbOutputCartons>>>> GetFbOutputCartonsList()
        {
            var response = new Services.ResponseModel<FbOutputCartons>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _FbOutputCartonsQueryRepository.GetAllAsync();
                response.Results = (List<FbOutputCartons>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("InsertFbOutputCartons")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<FbOutputCartons>>>> InsertFbOutputCartons([FromBody] FbOutputCartons insertFbOutputCartonsModel)
        {
            var response = new Services.ResponseModel<long>();

            try
            {
               
                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new CreateFbOutputCartonsQuery()
                {
                   
                   
                    AddedDate = DateTime.Now,
                    SessionId = Guid.NewGuid(),
                    LocationName = insertFbOutputCartonsModel.LocationName,
                    BatchNo = insertFbOutputCartonsModel.BatchNo,
                    ItemNo = insertFbOutputCartonsModel.ItemNo,
                    CartonNo= insertFbOutputCartonsModel.CartonNo,
                    Description = insertFbOutputCartonsModel.Description,
                    FullCarton = insertFbOutputCartonsModel.FullCarton,
                    LooseCartonQty= insertFbOutputCartonsModel.LooseCartonQty,
                    FullCartonQty = insertFbOutputCartonsModel.FullCartonQty,
                    AddedByUserID = insertFbOutputCartonsModel.AddedByUserID,
                    ProductionOrderNo = insertFbOutputCartonsModel.ProductionOrderNo,
                    PalletNo= insertFbOutputCartonsModel.PalletNo,
                    StatusCodeID = 1

                };

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }
        [HttpPost]
        [Route("UpdateFbOutputCartons")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<FbOutputCartons>>>> UpdateFbOutputCartons([FromBody] FbOutputCartons insertFbOutputCartonsModel)
        {
            var response = new Services.ResponseModel<long>();

            try
            {

                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new EditFbOutputCartonsQuery()
                {


                    ModifiedDate = DateTime.Now,
                   
                    LocationName = insertFbOutputCartonsModel.LocationName,
                    BatchNo = insertFbOutputCartonsModel.BatchNo,
                    ItemNo = insertFbOutputCartonsModel.ItemNo,
                    CartonNo = insertFbOutputCartonsModel.CartonNo,
                    Description = insertFbOutputCartonsModel.Description,
                    FullCarton = insertFbOutputCartonsModel.FullCarton,
                    LooseCartonQty = insertFbOutputCartonsModel.LooseCartonQty,
                    FullCartonQty = insertFbOutputCartonsModel.FullCartonQty,
                    ModifiedByUserID = insertFbOutputCartonsModel.ModifiedByUserID,
                    ProductionOrderNo = insertFbOutputCartonsModel.ProductionOrderNo,
                    PalletNo = insertFbOutputCartonsModel.PalletNo,
                    FbOutputCartonID = insertFbOutputCartonsModel.FbOutputCartonID

                };

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }

        [HttpPost]
        [Route("DeleteFbOutputCartons")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<FbOutputCartons>>>> DeleteFbOutputCartons([FromBody] FbOutputCartonsModels insertFbOutputCartonsModel)
        {
            var response = new Services.ResponseModel<long>();

            try
            {

                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new DeleteFbOutputCartonsQuery(insertFbOutputCartonsModel.FbOutputCartonID);
               

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }

        [HttpGet("GetFbOutputCartonsListCount")]
        public async Task<ActionResult<Services.ResponseModel<List<FbOutputCartons>>>> GetFbOutputCartonsListCount(string PalletNo)
        {
            var response = new Services.ResponseModel<FbOutputCartons>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _FbOutputCartonsQueryRepository.GetAllCartonsCountAsync(PalletNo);
                response.Results = (List<FbOutputCartons>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetFullCartonsList")]
        public async Task<ActionResult<Services.ResponseModel<List<FbOutputCartons>>>> GetFullCartonsList(string PalletNo)
        {
            var response = new Services.ResponseModel<FbOutputCartons>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _FbOutputCartonsQueryRepository.GetAllFullCartonsAsync(PalletNo);
                response.Results = (List<FbOutputCartons>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetLooseCartonsList")]
        public async Task<ActionResult<Services.ResponseModel<List<FbOutputCartons>>>> GetLooseCartonsList(string PalletNo)
        {
            var response = new Services.ResponseModel<FbOutputCartons>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _FbOutputCartonsQueryRepository.GetAllLooseCartonsAsync(PalletNo);
                response.Results = (List<FbOutputCartons>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpPost]
        [Route("InsertDispensedMeterial")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<DispensedMeterial>>>> InsertDispensedMeterial([FromBody] DispensedMeterial insertDispensedMeterialmodel)
        {
            var response = new Services.ResponseModel<long>();

            try
            {

                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new CreateDispensedMeterialQuery()
                {


                    AddedDate = DateTime.Now,
                    SessionId = Guid.NewGuid(),
                    MaterialName = insertDispensedMeterialmodel.MaterialName,
                    BatchNo = insertDispensedMeterialmodel.BatchNo,
                    QCReference = insertDispensedMeterialmodel.QCReference,
                    SubLotNo = insertDispensedMeterialmodel.SubLotNo,
                    Description = insertDispensedMeterialmodel.Description,
                    ProductionOrderNo = insertDispensedMeterialmodel.ProductionOrderNo,
                    TareWeight = insertDispensedMeterialmodel.TareWeight,
                    ActualWeight = insertDispensedMeterialmodel.ActualWeight,
                    AddedByUserID = insertDispensedMeterialmodel.AddedByUserID,
                    UOM = insertDispensedMeterialmodel.UOM,
                    PrintLabel= insertDispensedMeterialmodel.PrintLabel,
                    StatusCodeID = 1
                };

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }
        [HttpGet("GetDispensedMeterialList")]
        public async Task<ActionResult<Services.ResponseModel<List<DispensedMeterial>>>> GetDispensedMeterialList()
        {
            var response = new Services.ResponseModel<DispensedMeterial>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _FbOutputCartonsQueryRepository.GetAllDispensedMeterialAsync();
                response.Results = (List<DispensedMeterial>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpPost]
        [Route("InsertQCTimesheet")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<TimeSheetForQC>>>> InsertQCTimesheet([FromBody] TimeSheetForQC timeSheetForQC)
        {
            var response = new Services.ResponseModel<long>();

            try
            {

                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new CreateQCTimesheetQuery()
                {


                    AddedDate = DateTime.Now,
                    SessionId = Guid.NewGuid(),
                    ItemName = timeSheetForQC.ItemName,
                    RefNo = timeSheetForQC.RefNo,
                    Stage = timeSheetForQC.Stage,
                    TestName = timeSheetForQC.TestName,
                    QRcode = timeSheetForQC.QRcode,
                    DetailEntry = timeSheetForQC.DetailEntry,
                    Comment = timeSheetForQC.Comment,
                    SpecificTestName = timeSheetForQC.SpecificTestName,
                    AddedByUserID = timeSheetForQC.AddedByUserID,

                };

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }
        [HttpPost]
        [Route("UpdateQCTimesheet")]
        public async Task<ActionResult<Services.ResponseModel<IEnumerable<TimeSheetForQC>>>> UpdateQCTimesheet([FromBody] TimeSheetForQC timeSheetForQC)
        {
            var response = new Services.ResponseModel<long>();

            try
            {

                response.ResponseCode = Services.ResponseCode.Success;

                var lst = new UpdateQCTimesheetQuery()
                {

                    Comment = timeSheetForQC.Comment,
                   QCTimesheetID = timeSheetForQC.QCTimesheetID,
                    ModifiedDate = DateTime.Now,
                   ModifiedByUserID = timeSheetForQC.ModifiedByUserID

                };

                var Result = await _mediator.Send(lst);


                response.Result = Result;


            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.Result = 0;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);

        }
        [HttpGet("GetQCListByID")]
        public async Task<ActionResult<Services.ResponseModel<List<TimeSheetForQC>>>> GetQCListByID(long QCTimesheetID)
        {
            var response = new Services.ResponseModel<TimeSheetForQC>();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                var userNotifications = await _qcTimeSheetQueryRepository.GetAllQCTimeSheetAsync(QCTimesheetID);
                response.Results = (List<TimeSheetForQC>)userNotifications; // Assign the list of results
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpPost("UpdateTimeSheetForQC")]
        public async Task<ResponseModel> UpdateTimeSheetForQC([FromForm] Models.InsertTimeSheetQCModel value)
        {


            var response = new Services.ResponseModel<view_QCAssignmentRM>();
            ResponseModel fileresponse = new ResponseModel();
            string[] splitArray = value.ScanResult.Split('~');
            List<string> stringList = splitArray.ToList();


            if (stringList.Count == 2)
            {
                var list = await _qcTimeSheetQueryRepository.GetAllCompanyAsync(stringList[0], stringList[1], "Start");

                if (list.Count == 0)
                {
                    fileresponse.Message = "Please Start the Action.";

                }
                else
                {

                    foreach (var result in list)
                    {
                        var lst = new UpdateTimesheetQuery()
                        {


                            ModifiedDate = DateTime.Now,

                            ModifiedByUserID = value.UserID,
                            Action = value.Action,
                            MachineAction = value.MachineAction,
                            MachineName = value.MachineName,

                            EndDate = DateTime.Now,
                            QCTimesheetID = result.QCTimesheetID
                        };

                        var Result = await _mediator.Send(lst);
                    }
                    fileresponse.Message = "Action End Successfully!.";
                }
            }

            if (stringList.Count == 5)
            {
                var list = await _qcTimeSheetQueryRepository.GetAllItemAsync(stringList[0], stringList[1], stringList[3], "Start");
                if (list.Count == 0)
                {

                    fileresponse.Message = "Please Start the Action.";
                }
                else
                {

                    foreach (var result in list)
                    {
                        var lst = new UpdateTimesheetQuery()
                        {


                            ModifiedDate = DateTime.Now,

                            ModifiedByUserID = value.UserID,
                            Action = value.Action,
                            MachineAction = value.MachineAction,
                            MachineName = value.MachineName,

                            EndDate = DateTime.Now,
                            QCTimesheetID = result.QCTimesheetID
                        };

                        var Result = await _mediator.Send(lst);
                    }
                    fileresponse.Message = "Action End Successfully!.";
                }
            }

            
        
          
            return fileresponse;
        }
        public List<view_QCAssignmentRM> QCList { get; set; }
        [HttpPost("InsertTimeSheetForQC")]
        public async Task<ResponseModel> InsertTimeSheetForQC([FromForm] Models.InsertTimeSheetQCModel value)
        {
            DateTime? Date = null;
            string? Company = null;
            DateTime? Startdate = null;
            DateTime? Enddate = null;
            string? description = null;
            var response = new Services.ResponseModel<view_QCAssignmentRM>();
            ResponseModel fileresponse = new ResponseModel();
            string[] splitArray = value.ScanResult.Split('~');
            List<string> stringList = splitArray.ToList();
           

            if (stringList.Count == 2)
            {
               var list  = await  _qcTimeSheetQueryRepository.GetAllCompanyAsync(stringList[0], stringList[1], value.Action);
               
                if (list.Count == 0)
                {
                    QCList = (List<view_QCAssignmentRM>)await _qcTimeSheetQueryRepository.GetAllListByQRAsync(stringList[0], stringList[1]);
                    Date = DateTime.Parse(stringList[0]);
                    Company = stringList[1];
                }
               
                else
                {
                    
                    fileresponse.Message = "The Action Already Started.";
                }
               
            }

            if (stringList.Count == 5)
            {
                var list = await _qcTimeSheetQueryRepository.GetAllItemAsync(stringList[0], stringList[1], stringList[3],value.Action);
                if (list.Count == 0)
                {
                    QCList = (List<view_QCAssignmentRM>)await _qcTimeSheetQueryRepository.GetAllQCListByQRAsync(stringList[0], stringList[1], stringList[3]);
                    description = stringList[0];
                }
               
                else
                {
                   
                    fileresponse.Message = "The Scan Material Already Start.";
                }
            }

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;

                if (QCList != null)
                {

                    var Results = QCList;
                    // Assign the list of results
                    if (Results.Count > 0)
                    {
                        if (value.Action == "Start")
                        {
                            Startdate = DateTime.Now;
                        }
                        else
                        {
                            Enddate = DateTime.Now;
                        }

                        foreach (var result in Results)
                        {
                            var lst = new CreateQCTimesheetQuery()
                            {


                                AddedDate = DateTime.Now,
                                SessionId = Guid.NewGuid(),
                                ItemName = result.ItemNo,
                                RefNo = result.QCReferenceNo,
                                Stage = result.Person,
                                TestName = result.Test,
                                QRcode = true,
                                DetailEntry = result.Entry_ID,
                                Comment = value.Comment,
                                SpecificTestName = result.SpecificTest,
                                AddedByUserID = value.UserID,
                                Action = value.Action,
                                MachineAction = value.MachineAction,
                                MachineName = value.MachineName,
                                StartDate = Startdate,
                                EndDate = Enddate,
                                Date = Date,
                                Company = Company,
                                Description = description,
                            };

                            var Result = await _mediator.Send(lst);
                            var FileName = value.FileName;

                            if (FileName != null)
                            {

                                var QCsessionID = await _qcTimeSheetQueryRepository.GetAllQCTimeSheetAsync(Result);

                                if (!Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                                {
                                    fileresponse.IsSuccess = false;
                                    fileresponse.Message = "Invalid content type.";
                                    return fileresponse;
                                }

                                var file = Request.Form.Files;
                                if (file == null)
                                {
                                    fileresponse.IsSuccess = false;
                                    fileresponse.Message = "No file uploaded.";
                                    return fileresponse;
                                }


                                var sessionID = QCsessionID[0].SessionId;
                                var addedByUserId = value.UserID;
                                var SourceFrom = "FileProfile";
                                var ChangeNewFileName = value.FileName;
                                var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + sessionID + @"\";
                                string fileName = QCsessionID[0].SessionId + ".pdf";
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
                                        ProfileID = 0,
                                        PlantID = 0,
                                        DepartmentId = 0,
                                        SectionId = 0,
                                        SubSectionId = 0,
                                        DivisionId = 0,

                                    };
                                    var profileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
                                    Documents documents = new Documents();
                                    documents.UploadDate = DateTime.Now;
                                    documents.AddedByUserId = addedByUserId;
                                    documents.AddedDate = DateTime.Now;
                                    documents.SessionId = QCsessionID[0].SessionId;
                                    documents.IsLatest = true;
                                    documents.IsTemp = true;
                                    documents.FileName = ChangeNewFileName + ".pdf";
                                    documents.ContentType = "application/pdf";
                                    documents.FileSize = fileSize;
                                    documents.SourceFrom = SourceFrom;
                                    documents.ProfileNo = profileNo;
                                    documents.FilterProfileTypeId = 10078;
                                    documents.FilePath = serverFilePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                                    var responses = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                                    System.GC.Collect();
                                    GC.SuppressFinalize(this);
                                    fileresponse.IsSuccess = true;
                                    //response.Message = $"File uploaded successfully. Content Type: {contentType}, File size: {fileSize} bytes, File extension: {fileExtension}";

                                }
                            }

                            response.Results = Results;
                            fileresponse.Message = "Action Start Successfully!.";
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }
            return fileresponse;
        }
        [HttpGet("GetQCMachineStatus")]
        public async Task<ActionResult<Services.ResponseModel<List<View_ApplicationMasterDetail>>>> GetQCMachineStatus()
        {

            var response = new Services.ResponseModel<View_ApplicationMasterDetail>();

            var result = await _mediator.Send(new GetAllApplicationMasterDetailQuery(425));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<View_ApplicationMasterDetail> { new View_ApplicationMasterDetail() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetListFromQRCode")]
        public async Task<ActionResult<Services.ResponseModel<List<view_QCAssignmentRM>>>> GetListFromQRCode(string ScanResult)
        {
            var response = new Services.ResponseModel<view_QCAssignmentRM>();
            string[] splitArray = ScanResult.Split('~');
            List<string> stringList = splitArray.ToList();
            if(stringList.Count == 2)
            {
                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                    var userNotifications = await _qcTimeSheetQueryRepository.GetAllListByQRAsync(stringList[0], stringList[1]);
                    response.Results = (List<view_QCAssignmentRM>)userNotifications; // Assign the list of results
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }

            }

            if (stringList.Count == 5)
            {
                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                    var userNotifications = await _qcTimeSheetQueryRepository.GetAllQCListByQRAsync(stringList[0], stringList[1], stringList[3]);
                    response.Results = (List<view_QCAssignmentRM>)userNotifications; // Assign the list of results
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }

            }

            return Ok(response);
        }
    }
}

