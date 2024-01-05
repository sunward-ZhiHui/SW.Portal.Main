using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;
using Core.EntityModels;
using SW.Portal.Solutions.Models;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionActivitysController : Controller
    {
        private readonly IMediator _mediator;
        public ProductionActivitysController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("GetProductionActivityLists")]
        public async Task<ActionResult<ResponseModel<List<ProductActivitysAppModel>>>> GetProductionActivityLists(ProductActivityAppModel productActivityAppModel)
        {
            //ProductActivityAppModel productActivityAppModel = new ProductActivityAppModel();
            //productActivityAppModel.AddedByUserID = 1;
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
    }
}
