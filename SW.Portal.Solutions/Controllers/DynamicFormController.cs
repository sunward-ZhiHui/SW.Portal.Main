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
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormData>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId, Guid? DynamicFormDataSessionId, Guid? DynamicFormDataGridSessionId, Guid? DynamicFormSectionGridAttributeSessionId)
        {
            var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;
            var response = new Services.ResponseModel<DynamicFormData>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId, DynamicFormSectionGridAttributeSessionId, baseUrl));

            response.ResponseCode = Services.ResponseCode.Success;
            response.Results = result;
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
                                List<DynamicFormData> dynamicFormDatas = new List<DynamicFormData>();
                                var res = restApi(a.Url);
                                if (res.results.Count > 0)
                                {
                                    var counts = res.results.Count;

                                    List<object> Singlelists = new List<object>();
                                    for (int i = 0; i < counts; i++)
                                    {
                                        DynamicFormData dynamicFormData = res.results[i].ToObject<DynamicFormData>();

                                        if (res.results[i].objectDataList != null)
                                        {
                                            var itemValue = res.results[i].objectDataList;
                                            if (itemValue is JArray)
                                            {
                                                List<ExpandoObject> listData = itemValue.ToObject<List<ExpandoObject>>();
                                                if (listData != null && listData.Count > 0)
                                                {
                                                    var list = listData.FirstOrDefault().ToList();
                                                    if (list != null)
                                                    {
                                                        IDictionary<string, object> objectData = new ExpandoObject();
                                                        IDictionary<string, object> objectDataItems = new ExpandoObject();
                                                        IDictionary<string, object> objectDataSingleItems = new ExpandoObject();
                                                        objectDataSingleItems["dynamicFormDataId"] = dynamicFormData.DynamicFormDataId;
                                                        objectDataSingleItems["profileNo"] = dynamicFormData.ProfileNo;
                                                        objectDataSingleItems["name"] = dynamicFormData.Name;
                                                        objectDataSingleItems["SessionId"] = dynamicFormData.SessionId;
                                                        List<object> lists = new List<object>();
                                                        list.ForEach(k =>
                                                        {
                                                            dynamic val = k.Value;
                                                            objectData[k.Key] = k.Value;
                                                            objectDataItems[k.Key + "$" + val.Label.Replace(" ", "_")] = val.Value;
                                                            objectDataSingleItems[k.Key + "$" + val.Label.Replace(" ", "_")] = val.Value;
                                                        });
                                                        lists.Add(objectData);
                                                        Singlelists.Add(objectDataSingleItems);
                                                        dynamicFormData.ObjectDataItems = objectDataItems;
                                                        dynamicFormData.ObjectDataList = lists;
                                                        // dynamicFormData.GridSingleItems = Singlelists;
                                                    }
                                                }
                                            }
                                        }

                                        dynamicFormDatas.Add(dynamicFormData);
                                    }
                                    a.GridSingleItems = Singlelists;
                                    a.GridItems = dynamicFormDatas;
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
