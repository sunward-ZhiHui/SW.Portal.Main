using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;
using Core.EntityModels;
using SW.Portal.Solutions.Models;
using Core.Entities.Views;
using Core.Entities;
using Core.Repositories.Query;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionActivitysController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IProductionActivityQueryRepository _productionactivityQueryRepository;
        private readonly IRoutineQueryRepository _productionactivityRoutineQueryRepository;
        public ProductionActivitysController(IMediator mediator, IProductionActivityQueryRepository productionactivityQueryRepository, IRoutineQueryRepository productionactivityRoutineQueryRepository)
        {
            _mediator = mediator;
            _productionactivityQueryRepository = productionactivityQueryRepository;
            _productionactivityRoutineQueryRepository = productionactivityRoutineQueryRepository;
        }
        [HttpGet("GetApplicationMasterDetailList")]
        public async Task<ActionResult<ResponseModel<List<View_ApplicationMasterDetail>>>> GetApplicationMasterDetailList(long? Id)
        {
            var response = new ResponseModel<View_ApplicationMasterDetail>();
            var result = await _mediator.Send(new GetAllApplicationMasterDetailQuery(Id));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetProductionActivityLists")]
        public async Task<ActionResult<ResponseModel<List<ProductActivitysAppModel>>>> GetProductionActivityLists(long? UserId)
        {
            ProductActivityAppModel productActivityAppModel = new ProductActivityAppModel();
            productActivityAppModel.AddedByUserID = UserId;
            List<ProductActivitysAppModel> productActivityAppModels = new List<ProductActivitysAppModel>();
            var response = new ResponseModel<ProductActivitysAppModel>();
            List<ProductActivityAppModel> result = null;
            result = await _mediator.Send(new GetAllProductionActivityAppLineQuery(productActivityAppModel));
            if (result != null && result.Count() > 0)
            {
                result.ForEach(s =>
                {
                    ProductActivitysAppModel productActivityAppModel1 = new ProductActivitysAppModel();
                    productActivityAppModel1.ProductionActivityAppLineId = s.ProductionActivityAppLineId;
                    productActivityAppModel1.ProductionActivityAppId = s.ProductionActivityAppId;
                    productActivityAppModel1.CompanyId = s.CompanyId;
                    productActivityAppModel1.ProdActivityActionId = s.ProdActivityActionId;
                    productActivityAppModel1.ActionDropdown = s.ActionDropdown;
                    productActivityAppModel1.ProdActivityCategoryId = s.ProdActivityCategoryId;
                    productActivityAppModel1.ManufacturingProcessId = s.ManufacturingProcessId;
                    productActivityAppModel1.IsTemplateUpload = s.IsTemplateUpload;
                    productActivityAppModel1.ProdOrderNo = s.ProdOrderNo;
                    productActivityAppModel1.ProdActivityAction = s.ProdActivityAction;
                    productActivityAppModel1.ProdActivityCategory = s.ProdActivityCategory;
                    productActivityAppModel1.ManufacturingProcess = s.ManufacturingProcess;
                    productActivityAppModel1.IsTemplateUploadFlag = s.IsTemplateUploadFlag;
                    productActivityAppModel1.ProductActivityCaseLineId = s.ProductActivityCaseLineId;
                    productActivityAppModel1.NameOfTemplate = s.NameOfTemplate;
                    productActivityAppModel1.DocumentId = s.DocumentId;
                    productActivityAppModel1.Comment = s.Comment;
                    productActivityAppModel1.NavprodOrderLineId = s.NavprodOrderLineId;
                    productActivityAppModel1.LineSessionId = s.LineSessionId;
                    productActivityAppModel1.LineComment = s.LineComment;
                    productActivityAppModel1.Link = s.Link;
                    productActivityAppModel1.QaCheck = s.QaCheck;
                    productActivityAppModel1.RePlanRefNo = s.RePlanRefNo;
                    productActivityAppModel1.OrderLineNo = s.OrderLineNo;
                    productActivityAppModel1.ItemNo = s.ItemNo;
                    productActivityAppModel1.Description = s.Description;
                    productActivityAppModel1.Description1 = s.Description1;
                    productActivityAppModel1.BatchNo = s.BatchNo;
                    productActivityAppModel1.LocationToSaveId = s.LocationToSaveId;
                    productActivityAppModel1.OthersOptions = s.OthersOptions;
                    productActivityAppModel1.IsOthersOptions = s.IsOthersOptions;
                    productActivityAppModel1.DocumentParentId = s.DocumentParentId;
                    productActivityAppModel1.FileName = s.FileName;
                    productActivityAppModel1.ActivityProfileNo = s.ActivityProfileNo;
                    productActivityAppModel1.ProfileNo = s.ProfileNo;
                    productActivityAppModel1.ProfileId = s.ProfileId;
                    productActivityAppModel1.ContentType = s.ContentType;
                    productActivityAppModel1.IsLocked = s.IsLocked;
                    productActivityAppModel1.LockedByUserId = s.LockedByUserId;
                    productActivityAppModel1.LockedByUser = s.LockedByUser;
                    productActivityAppModel1.NotifyCount = s.NotifyCount;
                    productActivityAppModel1.ProdActivityResultId = s.ProdActivityResultId;
                    productActivityAppModel1.ProdActivityResult = s.ProdActivityResult;
                    productActivityAppModel1.StartDate = s.StartDate;
                    productActivityAppModel1.EndDate = s.EndDate;
                    productActivityAppModel1.TopicId = s.TopicId;
                    productActivityAppModel1.ManufacturingProcessChildId = s.ManufacturingProcessChildId;
                    productActivityAppModel1.ProdActivityCategoryChildId = s.ProdActivityCategoryChildId;
                    productActivityAppModel1.ProdActivityActionChildD = s.ProdActivityActionChildD;
                    productActivityAppModel1.ManufacturingProcessChild = s.ManufacturingProcessChild;
                    productActivityAppModel1.ProdActivityCategoryChild = s.ProdActivityCategoryChild;
                    productActivityAppModel1.ProdActivityActionChild = s.ProdActivityActionChild;
                    productActivityAppModel1.DocumentPermissionData = s.DocumentPermissionData;
                    productActivityAppModel1.ProductActivityPermissionData = s.ProductActivityPermissionData;
                    productActivityAppModel1.ProductActivityPermissions = s.ProductActivityPermissions;
                    productActivityAppModel1.ProdOrderNoDesc = s.ProdOrderNoDesc;
                    productActivityAppModel1.Type = s.Type;
                    productActivityAppModel1.QaCheckUserId = s.QaCheckUserId;
                    productActivityAppModel1.QaCheckDate = s.QaCheckDate;
                    productActivityAppModel1.QaCheckUser = s.QaCheckUser;
                    productActivityAppModel1.FilePath = s.FilePath;
                    productActivityAppModel1.UniqueSessionId = s.UniqueSessionId;
                    productActivityAppModel1.IsNewPath = s.IsNewPath;
                    productActivityAppModel1.LocationName = s.LocationName;
                    productActivityAppModel1.ProductionActivityAppLineQaCheckerModels = s.ProductionActivityAppLineQaCheckerModels;
                    productActivityAppModel1.LocationId = s.LocationId;
                    productActivityAppModel1.SupportDocCount = s.SupportDocCount;
                    productActivityAppModel1.ProductActivityCaseId = s.ProductActivityCaseId;
                    productActivityAppModel1.ResponsibilityUsers = s.ResponsibilityUsers;
                    productActivityAppModel1.RoutineStatusId = s.RoutineStatusId;
                    productActivityAppModel1.RoutineInfoIds = s.RoutineInfoIds;
                    productActivityAppModel1.ActivityMasterIds = s.ActivityMasterIds;
                    productActivityAppModel1.ActivityMaster = s.ActivityMaster;
                    productActivityAppModel1.ActivityResult = s.ActivityResult;
                    productActivityAppModel1.ActivityStatus = s.ActivityStatus;
                    productActivityAppModel1.ActivityMasterId = s.ActivityMasterId;
                    productActivityAppModel1.ActivityStatusId = s.ActivityStatusId;
                    productActivityAppModel1.CommentImage = s.CommentImage;
                    productActivityAppModel1.CommentImageType = s.CommentImageType;
                    productActivityAppModel1.IsEmailCreated = s.IsEmailCreated;
                    productActivityAppModel1.IsActionPermission = s.IsActionPermission;
                    productActivityAppModel1.NoAction = s.NoAction;
                    productActivityAppModel1.FileProfileTypeId = s.FileProfileTypeId;
                    productActivityAppModel1.IsDocuments = s.IsDocuments;
                    productActivityAppModel1.CompanyName = s.CompanyName;
                    productActivityAppModel1.ModifiedDate = s.ModifiedDate;
                    productActivityAppModel1.ModifiedByUser = s.ModifiedByUser;
                    productActivityAppModel1.StatusCodeID = s.StatusCodeID;
                    productActivityAppModel1.ModifiedByUserID = s.ModifiedByUserID;
                    productActivityAppModel1.ModifiedDate = s.ModifiedDate;
                    productActivityAppModel1.SessionId = s.SessionId;
                    productActivityAppModel1.AddedByUserID = s.AddedByUserID;
                    productActivityAppModel1.AddedDate = s.AddedDate;
                    productActivityAppModel1.AddedByUser = s.AddedByUser;
                    productActivityAppModel1.StatusCode = s.StatusCode;
                    productActivityAppModel1.IsPartialEmailCreated = s.IsPartialEmailCreated;
                    productActivityAppModel1.EmailActivitySessionId = s.EmailActivitySessionId;
                    productActivityAppModel1.EmailSessionId = s.EmailSessionId;
                    productActivityAppModels.Add(productActivityAppModel1);
                });
            }
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = productActivityAppModels;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpPost("UpdateActivityStatus")]
        public async Task<ActionResult<ResponseModel<ProductActivityAppStatusModel>>> UpdateActivityStatus(ProductActivityAppStatusModel value)
        {
            var response = new ResponseModel<ProductActivityAppStatusModel>();

            ProductActivityAppModel values = new ProductActivityAppModel();
            values.ProductionActivityAppLineId = value.ProductionActivityAppLineId;
            values.ActivityStatusId = value.ActivityStatusId;
            var result = await _mediator.Send(new UpdateActivityStatus(values));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Result = value;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetProductionActivityCheckedDetails")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityCheckedDetailsModel>>>> GetProductionActivityCheckedDetails(long? ProductionActivityAppLineId)
        {
            var response = new ResponseModel<ProductionActivityCheckedDetailsModel>();
            var result = await _mediator.Send(new GetProductionActivityCheckedDetails(ProductionActivityAppLineId));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpPost("InsertOrUpdateProductionActivityCheckedDetails")]
        public async Task<ActionResult<ResponseModel<ProductionActivityCheckedDetailsModel>>> InsertOrUpdateProductionActivityCheckedDetails(ProductionActivityCheckedDetailsModel value)
        {
            var response = new ResponseModel<ProductionActivityCheckedDetailsModel>();
            var result = await _mediator.Send(new InsertProductionActivityCheckedDetails(value));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Result = result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetProductionActivityEmail")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityAppLine>>>> GetProductionActivityEmail(long? ProductionActivityAppLineId)
        {


            var response = new ResponseModel<ProductionActivityAppLine>();
            var result = await _productionactivityQueryRepository.GetProductionActivityEmailList(ProductionActivityAppLineId);
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ProductionActivityAppLine>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }






        [HttpGet("GetAllProductionActivityRoutineAppLines")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityRoutineAppsModel>>>> GetAllProductionActivityRoutineAppLines(long? UserId)
        {
            ProductionActivityRoutineAppModel productActivityAppModel = new ProductionActivityRoutineAppModel();
            productActivityAppModel.AddedByUserID = UserId;
            List<ProductionActivityRoutineAppsModel> productActivityAppModels = new List<ProductionActivityRoutineAppsModel>();
            var response = new ResponseModel<ProductionActivityRoutineAppsModel>();
            List<ProductionActivityRoutineAppModel> result = null;
            result = await _mediator.Send(new GetAllProductionActivityRoutineAppLineQuery(productActivityAppModel));
            if (result != null && result.Count() > 0)
            {
                result.ForEach(s =>
                {
                    ProductionActivityRoutineAppsModel productActivityAppModel1 = new ProductionActivityRoutineAppsModel();
                    productActivityAppModel1.ProductionActivityRoutineAppLineId = s.ProductionActivityRoutineAppLineId;
                    productActivityAppModel1.ProductionActivityRoutineAppId = s.ProductionActivityRoutineAppId;
                    productActivityAppModel1.CompanyId = s.CompanyId;
                    productActivityAppModel1.ProdActivityActionId = s.ProdActivityActionId;
                    productActivityAppModel1.ActionDropdown = s.ActionDropdown;
                    productActivityAppModel1.ProdActivityCategoryId = s.ProdActivityCategoryId;
                    productActivityAppModel1.ManufacturingProcessId = s.ManufacturingProcessId;
                    productActivityAppModel1.IsTemplateUpload = s.IsTemplateUpload;
                    productActivityAppModel1.ProdOrderNo = s.ProdOrderNo;
                    productActivityAppModel1.ProdActivityAction = s.ProdActivityAction;
                    productActivityAppModel1.ProdActivityCategory = s.ProdActivityCategory;
                    productActivityAppModel1.ManufacturingProcess = s.ManufacturingProcess;
                    productActivityAppModel1.IsTemplateUploadFlag = s.IsTemplateUploadFlag;
                    productActivityAppModel1.ProductActivityCaseLineId = s.ProductActivityCaseLineId;
                    productActivityAppModel1.NameOfTemplate = s.NameOfTemplate;
                    productActivityAppModel1.DocumentID = s.DocumentId;
                    productActivityAppModel1.Comment = s.Comment;
                    productActivityAppModel1.NavprodOrderLineId = s.NavprodOrderLineId;
                    productActivityAppModel1.LineSessionId = s.LineSessionId;
                    productActivityAppModel1.LineComment = s.LineComment;
                    productActivityAppModel1.Link = s.Link;
                    productActivityAppModel1.QaCheck = s.QaCheck;
                    productActivityAppModel1.RePlanRefNo = s.RePlanRefNo;
                    productActivityAppModel1.OrderLineNo = s.OrderLineNo;
                    productActivityAppModel1.ItemNo = s.ItemNo;
                    productActivityAppModel1.Description = s.Description;
                    productActivityAppModel1.Description1 = s.Description1;
                    productActivityAppModel1.BatchNo = s.BatchNo;
                    productActivityAppModel1.LocationToSaveId = s.LocationToSaveId;
                    productActivityAppModel1.OthersOptions = s.OthersOptions;
                    productActivityAppModel1.IsOthersOptions = s.IsOthersOptions;
                    productActivityAppModel1.DocumentParentId = s.DocumentParentId;
                    productActivityAppModel1.FileName = s.FileName;
                    productActivityAppModel1.ActivityProfileNo = s.ActivityProfileNo;
                    productActivityAppModel1.ProfileNo = s.ProfileNo;
                    productActivityAppModel1.ProfileId = s.ProfileId;
                    productActivityAppModel1.ContentType = s.ContentType;
                    productActivityAppModel1.IsLocked = s.IsLocked;
                    productActivityAppModel1.LockedByUserId = s.LockedByUserId;
                    productActivityAppModel1.LockedByUser = s.LockedByUser;
                    productActivityAppModel1.NotifyCount = s.NotifyCount;
                    productActivityAppModel1.ProdActivityResultId = s.ProdActivityResultId;
                    productActivityAppModel1.ProdActivityResult = s.ProdActivityResult;
                    productActivityAppModel1.StartDate = s.StartDate;
                    productActivityAppModel1.EndDate = s.EndDate;
                    productActivityAppModel1.TopicId = s.TopicId;
                    productActivityAppModel1.ManufacturingProcessChildId = s.ManufacturingProcessChildId;
                    productActivityAppModel1.ProdActivityCategoryChildId = s.ProdActivityCategoryChildId;
                    productActivityAppModel1.ProdActivityActionChildD = s.ProdActivityActionChildD;
                    productActivityAppModel1.ManufacturingProcessChild = s.ManufacturingProcessChild;
                    productActivityAppModel1.ProdActivityCategoryChild = s.ProdActivityCategoryChild;
                    productActivityAppModel1.ProdActivityActionChild = s.ProdActivityActionChild;
                    productActivityAppModel1.DocumentPermissionData = s.DocumentPermissionData;
                    productActivityAppModel1.ProductActivityPermissionData = s.ProductActivityPermissionData;
                    productActivityAppModel1.ProductActivityPermissions = s.ProductActivityPermissions;
                    productActivityAppModel1.ProdOrderNoDesc = s.ProdOrderNoDesc;
                    productActivityAppModel1.Type = s.Type;
                    productActivityAppModel1.QaCheckUserId = s.QaCheckUserId;
                    productActivityAppModel1.QaCheckDate = s.QaCheckDate;
                    productActivityAppModel1.QaCheckUser = s.QaCheckUser;
                    productActivityAppModel1.FilePath = s.FilePath;
                    productActivityAppModel1.UniqueSessionId = s.UniqueSessionId;
                    productActivityAppModel1.IsNewPath = s.IsNewPath;
                    productActivityAppModel1.LocationName = s.LocationName;
                    productActivityAppModel1.ProductionActivityRoutineAppLineQaCheckerModels = s.ProductionActivityRoutineAppLineQaCheckerModels;
                    productActivityAppModel1.LocationId = s.LocationId;
                    productActivityAppModel1.SupportDocCount = s.SupportDocCount;
                    productActivityAppModel1.ProductActivityCaseId = s.ProductActivityCaseId;
                    productActivityAppModel1.ResponsibilityUsers = s.ResponsibilityUsers;
                    productActivityAppModel1.RoutineStatusId = s.RoutineStatusId;
                    productActivityAppModel1.RoutineInfoIds = s.RoutineInfoIds;
                    productActivityAppModel1.VisaMaster = s.VisaMaster;
                    productActivityAppModel1.ProdActivityResult = s.ProdActivityResult;
                    productActivityAppModel1.RoutineStatus = s.RoutineStatus;
                    productActivityAppModel1.VisaMasterId = s.VisaMasterId;
                    productActivityAppModel1.CommentImage = s.CommentImage;
                    productActivityAppModel1.CommentImageType = s.CommentImageType;
                    productActivityAppModel1.IsEmailCreated = s.IsEmailCreated;
                    productActivityAppModel1.IsActionPermission = s.IsActionPermission;
                    productActivityAppModel1.FileProfileTypeId = s.FileProfileTypeId;
                    productActivityAppModel1.IsDocuments = s.IsDocuments;
                    productActivityAppModel1.CompanyName = s.CompanyName;
                    productActivityAppModel1.ModifiedDate = s.ModifiedDate;
                    productActivityAppModel1.ModifiedByUser = s.ModifiedByUser;
                    productActivityAppModel1.StatusCodeID = s.StatusCodeID;
                    productActivityAppModel1.ModifiedByUserID = s.ModifiedByUserID;
                    productActivityAppModel1.ModifiedDate = s.ModifiedDate;
                    productActivityAppModel1.SessionId = s.SessionId;
                    productActivityAppModel1.AddedByUserID = s.AddedByUserID;
                    productActivityAppModel1.AddedDate = s.AddedDate;
                    productActivityAppModel1.AddedByUser = s.AddedByUser;
                    productActivityAppModel1.StatusCode = s.StatusCode;
                    productActivityAppModel1.IsPartialEmailCreated = s.IsPartialEmailCreated;
                    productActivityAppModel1.EmailActivitySessionId = s.EmailActivitySessionId;
                    productActivityAppModel1.EmailSessionId = s.EmailSessionId;
                    productActivityAppModels.Add(productActivityAppModel1);
                });
            }
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = productActivityAppModels;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpPost("UpdateActivityRoutineStatus")]
        public async Task<ActionResult<ResponseModel<ProductionActivityRoutineAppStatusModel>>> UpdateActivityRoutineStatus(ProductionActivityRoutineAppStatusModel value)
        {
            var response = new ResponseModel<ProductionActivityRoutineAppStatusModel>();

            ProductionActivityRoutineAppModel values = new ProductionActivityRoutineAppModel();
            values.ProductionActivityRoutineAppLineId = value.ProductionActivityRoutineAppLineId;
            values.RoutineStatusId = value.RoutineStatusId;
            values.ScreenID = ""; values.StatusCode = ""; values.AddedByUser = ""; values.ReferenceInfo = ""; values.Errormessage = ""; values.ModifiedByUser = "";
            var result = await _mediator.Send(new UpdateActivityRoutineStatus(values));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Result = value;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetProductionRoutineCheckedDetails")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityRoutineCheckedDetailsModel>>>> GetProductionRoutineCheckedDetails(long? ProductionActivityRoutineAppLineId)
        {
            var response = new ResponseModel<ProductionActivityRoutineCheckedDetailsModel>();
            var result = await _mediator.Send(new GetProductionRoutineCheckedDetails(ProductionActivityRoutineAppLineId));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpPost("InsertProductionRoutineCheckedDetails")]
        public async Task<ActionResult<ResponseModel<ProductionActivityRoutineCheckedDetailsModel>>> InsertProductionRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel value)
        {
            var response = new ResponseModel<ProductionActivityRoutineCheckedDetailsModel>();
            var result = await _mediator.Send(new InsertProductionRoutineCheckedDetails(value));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Result = result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetProductionActivityRoutineEmail")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityRoutineEmailModel>>>> GetProductionActivityRoutineEmail(long? ProductionActivityRoutineAppLineID)
        {

            var response = new ResponseModel<ProductionActivityRoutineEmailModel>();
         
            var result = await _productionactivityRoutineQueryRepository.GetProductionActivityRoutineEmailList(ProductionActivityRoutineAppLineID);
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ProductionActivityRoutineEmailModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetActivityReportList")]
        public async Task<ActionResult<Services.ResponseModel<List<View_ProductionActivityReport>>>> GetActivityReportList()
        {

            var response = new Services.ResponseModel<View_ProductionActivityReport>();

            var result = await _mediator.Send(new GetProductionActivityReportList());
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<View_ProductionActivityReport> { new View_ProductionActivityReport() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetActivityReportDocList")]
        public async Task<ActionResult<Services.ResponseModel<List<imgDocList>>>> GetActivityReportDocList(long ProductionActivityAppLineID)
        {

            var response = new Services.ResponseModel<imgDocList>();

            var result = await _mediator.Send(new GetProductionActivityReportDocumentList(ProductionActivityAppLineID));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result.Count > 0 ? result : new List<imgDocList> { new imgDocList() };
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
    }
}
