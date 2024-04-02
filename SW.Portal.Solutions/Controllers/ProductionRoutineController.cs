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
        public async Task<ActionResult<ResponseModel<List<ViewPlants>>>> GetCompanyList()
        {
            var response = new ResponseModel<ViewPlants>();
            var result = await _PlantQueryRepository.GetAllAsync();
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ViewPlants>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetLocation")]
        public async Task<ActionResult<ResponseModel<List<ProductionActivityApp>>>> GetLocation(long? CompanyID)
        {

            var response = new ResponseModel<ProductionActivityApp>();

            var result = await _ProductionActivityAppQueryRepository.GetAllAsync(CompanyID);
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ProductionActivityApp>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetRoutineList")]
        public async Task<ActionResult<ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineList()
        {

            var response = new ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildListQuery("108"));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineCategoryList")]
        public async Task<ActionResult<ResponseModel<List<ApplicationMasterChildModel>>>> GetRoutineCategoryList(long ManufacturingProcessChildId)
        {

            var response = new ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ManufacturingProcessChildId));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetActionList")]
        public async Task<ActionResult<ResponseModel<List<ApplicationMasterChildModel>>>> GetActionList(long ProdActivityCategoryChildId)
        {

            var response = new ResponseModel<ApplicationMasterChildModel>();

            var result = await _mediator.Send(new GetAllApplicationMasterChildByIdQuery(ProdActivityCategoryChildId));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<ApplicationMasterChildModel>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        [HttpGet("GetRoutineInfo")]
        public async Task<ActionResult<ResponseModel<List<View_ApplicationMasterDetail>>>> GetRoutineInfo()
        {

            var response = new ResponseModel<View_ApplicationMasterDetail>();

            var result = await _mediator.Send(new GetAllApplicationMasterDetailQuery(331));
            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = (List<View_ApplicationMasterDetail>)result;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetTemplateList")]
        public async Task<ActionResult<ResponseModel<List<ProductActivityCaseLineModel>>>> GetTemplateList(long ManufacturingProcessChildId, long ProdActivityCategoryChildId,long ProdActivityActionChildD)
        {

            var response = new ResponseModel<ProductActivityCaseLineModel>();

            if (ManufacturingProcessChildId > 0 && ProdActivityCategoryChildId > 0)
            {
                var result = await _mediator.Send(new GetProductActivityCaseLineTemplateItems(ManufacturingProcessChildId, ProdActivityCategoryChildId, ProdActivityActionChildD));


                try
                {
                    response.ResponseCode = ResponseCode.Success;
                    response.Results = (List<ProductActivityCaseLineModel>)result;
                }
                catch (Exception ex)
                {
                    response.ResponseCode = ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }
            }
            return Ok(response);
        }
        [HttpPost("InsertRoutineMaster")]
        public async Task<ActionResult<ResponseModel<ProductionActivityRoutineAppModel>>> InsertRoutineMaster(ProductionActivityRoutineAppModel value)
        {
            var response = new ResponseModel<ProductionActivityRoutineAppModel>();
            var request = new CreateProductionActivityRoutineAppCommand
            {
               // ProductionActivityRoutineAppLineId = ProductionActivityRoutineAppLineId,
                CompanyId = value.CompanyID,
                ProdOrderNo = value.ProdOrderNo,
                LocationId = value.LocationID,
              //  AddedDate = _selectedEditItems != null ? _selectedEditItems.AddedDate : DateTime.Now,
              // SessionId = _selectedEditItems != null ? _selectedEditItems.SessionId : Guid.NewGuid(),
               // LineSessionId = _selectedEditItems != null ? _selectedEditItems.LineSessionId : Guid.NewGuid(),
               // StatusCodeID = _selectedEditItems != null ? _selectedEditItems.StatusCodeID : 1,
               // AddedByUserID = _selectedEditItems != null ? _selectedEditItems.AddedByUserID : applicationUser.UserID,
                ManufacturingProcessChildId = value.ManufacturingProcessChildId,
                ProdActivityCategoryChildId = value.ProdActivityCategoryChildId,
                ProdActivityActionChildD = value.ProdActivityActionChildD,
                ProdActivityResultId = value.ProdActivityResultId,
                RoutineStatusId = value.RoutineStatusId,
                LineComment = value.LineComment,
                NavprodOrderLineId = value.NavprodOrderLineId > 0 ? value.NavprodOrderLineId : null,
                ModifiedByUserID = value.AddedByUserID,
                ModifiedDate = DateTime.Now,
                IsOthersOptions = value.OthersOptions == "Yes" ? true : false,
                //IsOthersOptions=Data.IsOthersOptions,
                IsTemplateUpload = value.IsTemplateUpload,
                IsTemplateUploadFlag = value.IsTemplateUpload == true ? "Yes" : "No",
                ProductActivityCaseLineId = value.ProductActivityCaseLineId > 0 ? value.ProductActivityCaseLineId : null,
                RoutineInfoIds = value.RoutineInfoIds,
            };

            var result = await _mediator.Send(request);
            try
            {
                response.ResponseCode = ResponseCode.Success;
                //response.Result = result;
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
