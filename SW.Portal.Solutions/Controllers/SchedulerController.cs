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
using DocumentFormat.OpenXml.Spreadsheet;
namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : Controller
    {
        private readonly IMediator _mediator;
        public SchedulerController(IMediator mediator)
        {
            _mediator = mediator;

        }
        public class ReturnItems
        {
            public List<ResourceData> Items { get; set; } = new List<ResourceData>();
            public int Count { get; set; } = 0;
        }
        [HttpGet("GetGanttList")]
        public async Task<ActionResult<ReturnItems>> GetGanttList()
        {
            ReturnItems returnItems = new ReturnItems();
            var result = await _mediator.Send(new GetSchedulerResourceData());
            var tempList = new List<ResourceData>();
            int idCounter = 1;

            if (result?.Any() == true)
            {
                // group by Type for easy lookup
                var groupedData = result
                    .OrderBy(o => o.Id)
                    .GroupBy(x => x.Type)
                    .ToDictionary(g => g.Key, g => g.ToList());

                if (groupedData.TryGetValue("Main", out var mainProjects))
                {
                    foreach (var main in mainProjects)
                    {
                        main.Subject = main.Text;
                        main.ChildId = idCounter++;
                        main.ParentId = null; // top-level
                        tempList.Add(main);

                        if (groupedData.TryGetValue("Parent", out var parentProjects))
                        {
                            var parents = parentProjects
                                .Where(p => p.PlanningForProductionProcessByMachine == main.PlanningForProductionProcessByMachine);

                            foreach (var parent in parents)
                            {
                                parent.Subject = parent.Text;
                                parent.ChildId = idCounter++;
                                parent.ParentId = main.ChildId; // link to Main
                                tempList.Add(parent);

                                if (groupedData.TryGetValue("Event", out var eventProjects))
                                {
                                    var events = eventProjects
                                        .Where(e => e.ProductionPlanningSchedulerId == parent.ProductionPlanningSchedulerId);

                                    foreach (var ev in events)
                                    {
                                        //ev.Subject = ev.Text;
                                        ev.ChildId = idCounter++;
                                        ev.ParentId = parent.ChildId; // link to Parent
                                        tempList.Add(ev);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            returnItems.Items = tempList;
            returnItems.Count = tempList.Count();
            return returnItems;
        }
    }


}
