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
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormData>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId)
        {
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            //var baseUrl =  HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormData>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, baseUrl));
            if (result.Count > 0)
            {
                result.ForEach(s =>
                {
                    if (s.DynamicFormReportItems.Count > 0)
                    {
                        s.DynamicFormReportItems.ForEach(a =>
                        {
                            if (a.IsGrid == true)
                            {
                                var res = restApi(a.Url);
                                if (res.results.Count > 0)
                                {
                                    List<DynamicFormData> listData = res.results.ToObject<List<DynamicFormData>>();
                                    a.GridItems = listData;
                                }
                            }
                        });
                    }
                });
            }
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
    }
}
