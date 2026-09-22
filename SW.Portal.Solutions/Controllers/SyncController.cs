using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SW.Portal.Solutions.Services;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : Controller
    {
        private readonly IProductionSimulationQueryRepository _productionSimulationQueryRepository;
        private readonly IMediator _mediator;
        public SyncController(IProductionSimulationQueryRepository productionSimulationQueryRepository, IMediator mediator)
        {
            _productionSimulationQueryRepository = productionSimulationQueryRepository;
            _mediator = mediator;
        }

        [HttpGet("SyncProductionSimulation")]

        public async Task<ActionResult<ResponseModel<List<ProductionSimulation>>>> SyncProductionSimulation()
        {
            var response = new ResponseModel<ProductionSimulation>();
            var result = (await _productionSimulationQueryRepository.GetProductionSimulationListAsync(1, true, 588)).ToList();

            response.Result = result.FirstOrDefault() ?? new();
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }
            return Ok(response);
        }


        [HttpGet("SyncNavMonthStockBalance")]
        public async Task<ActionResult<ResponseModel<string>>> SyncNavMonthStockBalance()
        {
            var response = new ResponseModel<string>();
            StockBalanceSearch stockBalanceSearch = new StockBalanceSearch();
            stockBalanceSearch.UserId = 588;
            stockBalanceSearch.StkMonth = DateTime.Today;
            stockBalanceSearch.CompanyId = 1;
            var result = await _mediator.Send(new GetNAVStockBalanceQuery(stockBalanceSearch));
            response.Result = result;
            try
            {
                response.ResponseCode = ResponseCode.Success;
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
