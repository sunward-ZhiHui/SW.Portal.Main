using AC.SD.Core.Data;
using AC.SD.Core.Pages.Masters;
using Application.Queries;
using Application.Queries.Base;
using Azure.Core.Serialization;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using DevExpress.CodeParser;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Web;
using DevExpress.Xpo;
using DevExpress.XtraSpreadsheet.TileLayout;
using Google.Api.Gax.ResourceNames;
using Infrastructure.Repository.Query;
using MediatR;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Edm.Values;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;
using System.Data;
using System.Dynamic;
using System.Linq;
using Method = RestSharp.Method;
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
        /* [HttpGet("GetDynamicFormAttributeItemList")]

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
         }*/

        [HttpGet("GetTimeSheetDropDownList")]
        public async Task<ActionResult<Services.ResponseModel<List<DropDownOptionsListModel>>>> GetTimeSheetDropDownList(int? attrId)
        {
            List<object> attributeDetails = new List<object>();
            IDictionary<string, object> dynamicsData2 = new ExpandoObject();
            dynamicsData2[108.ToString()] = 8;
            dynamicsData2[109.ToString()] = 2273;
            dynamicsData2[112.ToString()] = attrId??-1;
            var response = new Services.ResponseModel<DropDownOptionsListModel>();
            var result = await _mediator.Send(new GetApplicationMasterParentByList(dynamicsData2, 108));

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;

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
            List<DynamicFormFilterOdata> dynamicFormFilterOdatas=new List<DynamicFormFilterOdata>();
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormDataResponse>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true, null, null, dynamicFormFilterOdatas));

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
            List<string> DynamicFormDataDefList = new List<string>() { "PageSize", "pageNo", "DynamicFormSessionId", "DynamicFormDataSessionId", "DynamicFormDataGridSessionId", "DynamicFormSectionGridAttributeSessionId" };
            List<DynamicFormFilterOdata> DynamicFormFilterOdatas = new List<DynamicFormFilterOdata>();
            int? PageNo = 1; int? PageSize = 50;
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var query = HttpContext.Request.Query.ToList();
            if (query != null && query.Count > 0)
            {
                DynamicFormDataDefList = DynamicFormDataDefList.Select(s => s.ToLower()).ToList();
                query.ForEach(q =>
                {
                    if (!string.IsNullOrEmpty(q.Key))
                    {
                        if (!DynamicFormDataDefList.Contains(q.Key.ToLower()))
                        {
                            var values = (string?)q.Value;
                            if (!string.IsNullOrEmpty(values))
                            {
                                DynamicFormFilterOdata dynamicFormFilterOdata = new DynamicFormFilterOdata();
                                dynamicFormFilterOdata.Key = q.Key;
                                dynamicFormFilterOdata.Value = (string?)q.Value;
                                DynamicFormFilterOdatas.Add(dynamicFormFilterOdata);
                            }
                        }
                    }

                });
            }
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
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true, PageNo, PageSize, DynamicFormFilterOdatas));
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
