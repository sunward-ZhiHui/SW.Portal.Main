using AC.SD.Core.Data;
using Application.Queries;
using Application.Queries.Base;
using ChartJs.Blazor.ChartJS.Common.Axes;
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
    }
}

