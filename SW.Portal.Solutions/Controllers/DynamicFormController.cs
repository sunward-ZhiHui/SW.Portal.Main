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
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicFormController : Controller
    {
        private readonly IMediator _mediator;
        public DynamicFormController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpGet("GetDynamicFormDataList")]
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormDataResponse>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {

            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormDataResponse>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl, true));

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

        /* [HttpGet("GetDynamicFormDataList")]
         public async Task<ActionResult<Services.ResponseModel<List<ExpandoObject>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
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
 */
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
