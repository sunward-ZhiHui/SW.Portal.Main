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
        public async Task<ActionResult<Services.ResponseModel<List<DynamicFormData>>>> GetDynamicFormDataList(Guid? DynamicFormSessionId,Guid? DynamicFormDataSessionId,Guid? DynamicFormDataGridSessionId)
        {
            var response = new Services.ResponseModel<DynamicFormData>();
            var result = await _mediator.Send(new GetDynamicFormApi(DynamicFormSessionId, DynamicFormDataSessionId, DynamicFormDataGridSessionId));
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
