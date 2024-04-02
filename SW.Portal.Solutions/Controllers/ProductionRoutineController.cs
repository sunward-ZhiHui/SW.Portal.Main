using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;
using Core.EntityModels;
using SW.Portal.Solutions.Models;
using Core.Entities.Views;
using Core.Entities;
using Core.Repositories.Query;
using AC.SD.Core.Data;
using AC.SD.Core.Pages.Masters;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionRoutineController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IPlantQueryRepository _PlantQueryRepository;
        private readonly IProductionActivityAppQueryRepository _ProductionActivityAppQueryRepository;

        public ProductionRoutineController(IMediator mediator, IPlantQueryRepository PlantQueryRepository, IProductionActivityAppQueryRepository productionActivityAppQueryRepository)
        {
            _mediator = mediator;
            _PlantQueryRepository = PlantQueryRepository;
            _ProductionActivityAppQueryRepository = productionActivityAppQueryRepository;
        }
        [HttpGet("GetCompanyList")]
        public async Task<ActionResult<Services.ResponseModel<List<ViewPlants>>>> GetCompanyList()
        {
            var response = new Services.ResponseModel<ViewPlants>();
            var result = await _PlantQueryRepository.GetAllAsync();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = (List<ViewPlants>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetLocation")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductionActivityAppModel>>>> GetLocation(long? CompanyID)
        {

            var response = new Services.ResponseModel<ProductionActivityAppModel>();

            var result = await _ProductionActivityAppQueryRepository.GetAllAsync(CompanyID);
            var displayResult = result?.Select(topic => new ProductionActivityAppModel
            {
               ProductionActivityAppID =topic.ProductionActivityAppID,
                CompanyID =topic.CompanyID,
                LocationID = topic.LocationID,
                ProdOrderNo = topic.ProdOrderNo,
                Comment = topic.Comment,
                ICTMasterID =topic.ICTMasterID,
                TopicID =topic.TopicID,
                DeropdownName = topic.DeropdownName,
            }).ToList();

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = displayResult;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetRoutineList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineList()
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildListQuery("108"));
            try
            {
                response.ResponseCode = Services. ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services. ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineCategoryList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineCategoryList(long ManufacturingProcessChildId)
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ManufacturingProcessChildId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetActionList")]
        public async Task<ActionResult<Services.ResponseModel<List<ApplicationMasterChildModel>>>> GetActionList(long ProdActivityCategoryChildId)
        {

            var response = new Services.ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ProdActivityCategoryChildId));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineInfo")]
        public async Task<ActionResult<Services.ResponseModel<List<View_ApplicationMasterDetail>>>> GetRoutineInfo()
        {

            var response = new Services.ResponseModel<View_ApplicationMasterDetail>();

            var result = await _mediator.Send(new GetAllApplicationMasterDetailQuery(331));
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = (List<View_ApplicationMasterDetail>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetTemplateList")]
        public async Task<ActionResult<Services.ResponseModel<List<ProductActivityCaseLineModel>>>> GetTemplateList(long ManufacturingProcessChildId, long ProdActivityCategoryChildId,long ProdActivityActionChildD)
        {

            var response = new Services.ResponseModel<ProductActivityCaseLineModel>();

            if (ManufacturingProcessChildId > 0 && ProdActivityCategoryChildId > 0)
            {
                var result = await _mediator.Send(new GetProductActivityCaseLineTemplateItems(ManufacturingProcessChildId, ProdActivityCategoryChildId, ProdActivityActionChildD));


                try
                {
                    response.ResponseCode = Services.ResponseCode.Success;
                    response.Results = (List<ProductActivityCaseLineModel>)result;
                }
                catch (Exception ex)
                {
                    response.ResponseCode = Services.ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }
            }
            return Ok(response);
        }
        [HttpPost("InsertRoutineMaster")]
        public async Task<ActionResult<Services.ResponseModel<ProductionActivityRoutineAppModel>>> InsertRoutineMaster(ProductionActivityRoutineAppModel value)
        {
            var response = new Services.ResponseModel<ProductionActivityRoutineAppModel>();
            var request = new CreateProductionActivityRoutineAppCommand

            {
                ProductionActivityRoutineAppLineId = 0,
                CompanyId = value.CompanyID,
                ProdOrderNo = value.ProdOrderNo,
                LocationId = value.LocationID,
                AddedDate = DateTime.Now,
                SessionId = Guid.NewGuid(),
                LineSessionId = Guid.NewGuid(),
                StatusCodeID = 1,
                AddedByUserID = value.AddedByUserID,
                ManufacturingProcessChildId = value.ManufacturingProcessChildId,
                ProdActivityCategoryChildId = value.ProdActivityCategoryChildId,
                ProdActivityActionChildD = value.ProdActivityActionChildD,
                ProdActivityResultId = value.ProdActivityResultId,
                RoutineStatusId = value.RoutineStatusId,
                LineComment = value.LineComment,
                NavprodOrderLineId = value.NavprodOrderLineId > 0 ? value.NavprodOrderLineId : null,
                // ModifiedByUserID = value.AddedByUserID,
                //  ModifiedDate = DateTime.Now,
                IsOthersOptions = value.OthersOptions == "Yes" ? true : false,

                IsTemplateUpload = value.IsTemplateUpload,
                IsTemplateUploadFlag = value.IsTemplateUpload == true ? "Yes" : "No",
                ProductActivityCaseLineId = value.ProductActivityCaseLineId > 0 ? value.ProductActivityCaseLineId : null,
                RoutineInfoIds = value.RoutineInfoIds,
            };

            var result = await _mediator.Send(request);
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Result = request;
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
