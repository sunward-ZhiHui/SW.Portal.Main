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
using Google.Api.Gax.ResourceNames;
using DevExpress.Web;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using Newtonsoft.Json;
using DevExpress.Xpo;
using Application.Queries.Base;
using Infrastructure.Repository.Query;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using DevExpress.XtraSpreadsheet.TileLayout;
using System.Dynamic;
using Microsoft.Data.Edm.Values;
using System.Data;
using Newtonsoft.Json.Converters;
using DevExpress.CodeParser;
using Newtonsoft.Json.Linq;
using Method = RestSharp.Method;
using Microsoft.Ajax.Utilities;
using Core.Repositories.Query.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq;
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicFormController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDynamicFormOdataQueryRepository _dynamicFormOdataQueryRepository;
        public DynamicFormController(IMediator mediator, IDynamicFormOdataQueryRepository dynamicFormOdataQueryRepository)
        {
            _mediator = mediator;
            _dynamicFormOdataQueryRepository = dynamicFormOdataQueryRepository;

        }
        [HttpGet("GetDynamicFormDropdownList")]

        public async Task<ActionResult<Services.ResponseModel<List<object>>>> GetDynamicFormDropdownList(Guid? DynamicFormSessionId, string? attrName)
        {
            List<object> attributeDetails = new List<object>();
            var response = new Services.ResponseModel<object>();
            var _dynamicForm = await _mediator.Send(new GetAllDynamicFormList(DynamicFormSessionId, -1));
            if (_dynamicForm != null && _dynamicForm.ID > 0)
            {
                var result = await _dynamicFormOdataQueryRepository.GetDropdownList(_dynamicForm.ID, attrName);
                if (result != null && result.Count() > 0)
                {
                    attributeDetails = result.ToList();
                }
            }
            response.Results = attributeDetails;
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
        [HttpGet("GetDynamicFormAttributeItemList")]

        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormSectionAttributesList>>>> GetDynamicFormAttributeItemList(Guid? DynamicFormSessionId)
        {
            List<DynamicFormSectionAttributesList> dynamicFormSectionAttributesLists = new List<DynamicFormSectionAttributesList>();
            var response = new Services.ResponseModel<DynamicFormSectionAttributesList>();
            var _dynamicForm = await _mediator.Send(new GetAllDynamicFormList(DynamicFormSessionId, -1));
            if (_dynamicForm != null && _dynamicForm.ID > 0)
            {
                var result = await _dynamicFormOdataQueryRepository.GetDynamicFormSectionAttributeList(_dynamicForm.ID);
                if (result != null && result.Count() > 0)
                {
                    dynamicFormSectionAttributesLists = result.ToList();
                }
            }
            response.Results = dynamicFormSectionAttributesLists;
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
        [HttpGet("GetDynamicFormDataList")]

        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormDataResponse>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {

            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormDataResponse>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true, null, null));

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetDynamicFormDataListPaging")]
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormDataResponse>>>> GetDynamicFormDataListPaging(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {
            int? PageNo = 1; int? PageSize = 50;
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var query = HttpContext.Request.Query.ToList();
            var PageNoExis = query.Where(w => w.Key.ToLower() == "pageNo".ToLower()).FirstOrDefault().Key;
            if (PageNoExis != null)
            {
                var PageNos = query.Where(w => w.Key.ToLower() == "pageNo".ToLower()).FirstOrDefault().Value;
                if (!string.IsNullOrEmpty(PageNos))
                {
                    decimal pNo = decimal.Parse(PageNos);
                    if (pNo > 0)
                    {
                        PageNo = (int?)Math.Round(pNo);
                    }
                }
            }
            var PageSizeExis = query.Where(w => w.Key.ToLower() == "PageSize".ToLower()).FirstOrDefault().Key;
            if (PageSizeExis != null)
            {
                var PageSizes = query.Where(w => w.Key.ToLower() == "PageSize".ToLower()).FirstOrDefault().Value;
                if (!string.IsNullOrEmpty(PageSizes))
                {
                    decimal pNo = decimal.Parse(PageSizes);
                    if (pNo > 0)
                    {
                        PageSize = (int?)Math.Round(pNo);
                    }
                }
            }
            var response = new Services.ResponseModel<DynamicFormDataResponse>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true, PageNo, PageSize));
            /* PageNo = PageNo > 0 ? PageNo : 1;
             var finalresult = result.Skip((int)((PageNo - 1) * 20))
                 .Take(20)
                 .ToList();*/
            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetDynamicFormDataListItem")]
        public async Task<ActionResult<Services.ResponseModel<List<ExpandoObject>>>> GetDynamicFormDataListItem(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {

            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<ExpandoObject>();
            var result = await _mediator.Send(new GetDynamicFormObjectsApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true));

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetDynamicFormAttributeList")]
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormDataResponse>>>> GetDynamicFormAttributeList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormDataResponse>();
            var result = await _mediator.Send(new GetDynamicFormAttributeApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true));

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;
            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result;

            }
            catch (Exception ex)
            {
                response.ResponseCode = Services.ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        private dynamic restApi(string? Url)
        {
            var options = new RestClientOptions
            {
                //MaxTimeout = 5 * 1000,
            };
            var client = new RestClient(options);
            var request = new RestRequest(Url, Method.Get);
            RestResponse response = client.Get(request);
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
        [HttpGet("GetQcTestRequirementSummery")]
        public async Task<ActionResult<Services.ResponseModel<List<QCTestRequirement>>>> GetQcTestRequirementSummery()
        {
            var response = new Services.ResponseModel<QCTestRequirement>();
            var result = await _mediator.Send(new GetQcTestRequirementSummery());

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;

            try
            {
                response.ResponseCode = Services.ResponseCode.Success;
                response.Results = result;

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
