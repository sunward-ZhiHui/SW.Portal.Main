using AC.SD.Core.Data;
using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using Infrastructure.Repository.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAppController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IFbOutputCartonsQueryRepository _FbOutputCartonsQueryRepository;
        public MobileAppController(IMediator mediator, IFbOutputCartonsQueryRepository FbOutputCartonsQueryRepository)
        {
            _mediator = mediator;
            _FbOutputCartonsQueryRepository = FbOutputCartonsQueryRepository;
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
    }
}

