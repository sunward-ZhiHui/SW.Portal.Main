using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Core.Repositories.Query;
using Core.Entities.Views;
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavFunController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IPlantQueryRepository _plantQueryRepository;
        public NavFunController(IMediator mediator, IPlantQueryRepository plantQueryRepository)
        {
            _mediator = mediator;
            _plantQueryRepository = plantQueryRepository;

        }
        private async Task<List<ViewPlants>> GetPlatDatas()
        {
            List<ViewPlants> viewPlants = new List<ViewPlants>();
            var plantData = await _plantQueryRepository.GetAllByNavCompanyAsync();
            List<FinishedProdOrderLine> finishedProdOrderLines = new List<FinishedProdOrderLine>();
            List<string> NavCompanyName = new List<string>() { "NAV_JB", "NAV_SG" };
            if (plantData != null && plantData.Count() > 0)
            {
                viewPlants = plantData.Where(w => w.NavCompanyName != null && w.NavCompanyName != "" && NavCompanyName.Contains(w.NavCompanyName)).ToList();
            }
            return viewPlants;
        }
        [HttpGet("InsertOrUpdateFinishedProdOrderLineData")]
        public async Task<ActionResult<Services.ResponseModel<List<FinishedProdOrderLine>>>> InsertOrUpdateFinishedProdOrderLineData()
        {
            var response = new Services.ResponseModel<FinishedProdOrderLine>();
            List<FinishedProdOrderLine> finishedProdOrderLines = new List<FinishedProdOrderLine>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var item in plantDatas)
                {
                    await _mediator.Send(new GetFinishedProdOrderLineQuery(item.PlantID));
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = finishedProdOrderLines;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = finishedProdOrderLines;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("InsertOrUpdateItemBatch")]
        public async Task<ActionResult<Services.ResponseModel<List<ItemBatchInfo>>>> InsertOrUpdateItemBatch()
        {
            var response = new Services.ResponseModel<ItemBatchInfo>();
            List<ItemBatchInfo> itemBatchInfo = new List<ItemBatchInfo>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var item in plantDatas)
                {
                    await _mediator.Send(new NavCompanyItemBatchInfoQuery(item.PlantID));
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = itemBatchInfo;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = itemBatchInfo;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("InsertOrUpdateNavprodOrderLine")]
        public async Task<ActionResult<Services.ResponseModel<List<NavprodOrderLine>>>> InsertOrUpdateNavprodOrderLine()
        {
            var response = new Services.ResponseModel<NavprodOrderLine>();
            List<NavprodOrderLine> itemBatchInfo = new List<NavprodOrderLine>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var item in plantDatas)
                {
                    await _mediator.Send(new GetNavprodOrderLineListQuery(item.PlantID));
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = itemBatchInfo;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = itemBatchInfo;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("InsertOrUpdateNavVendor")]
        public async Task<ActionResult<Services.ResponseModel<List<SoCustomer>>>> InsertOrUpdateNavVendor()
        {
            var response = new Services.ResponseModel<SoCustomer>();
            List<SoCustomer> NavVendors = new List<SoCustomer>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var item in plantDatas)
                {
                    await _mediator.Send(new InsertOrUpdateNavVendor(item.PlantID));
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = NavVendors;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = NavVendors;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("InsertOrUpdateNavItems")]
        public async Task<ActionResult<Services.ResponseModel<List<Navitems>>>> InsertOrUpdateNavItems()
        {
            var response = new Services.ResponseModel<Navitems>();
            List<Navitems> Navitems = new List<Navitems>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var item in plantDatas)
                {
                    await _mediator.Send(new NavCompanyItemQuery(item.PlantID, 1));
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = Navitems;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = Navitems;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("InsertOrUpdateRawItems")]
        public async Task<ActionResult<Services.ResponseModel<List<Navitems>>>> InsertOrUpdateRawItems()
        {
            List<string> Types = new List<string>() { "RawMatItem", "PackagingItem", "ProcessItem" };
            var response = new Services.ResponseModel<Navitems>();
            List<Navitems> Navitems = new List<Navitems>();
            var plantDatas = await GetPlatDatas();
            if (plantDatas != null && plantDatas.Count() > 0)
            {
                foreach (var Typesitem in Types)
                {
                    foreach (var item in plantDatas)
                    {
                        await _mediator.Send(new rawitemSalesOrderQuery(item.NavCompanyName, item.PlantID, Typesitem));
                    }
                }
            }
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = Navitems;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = Navitems;

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
