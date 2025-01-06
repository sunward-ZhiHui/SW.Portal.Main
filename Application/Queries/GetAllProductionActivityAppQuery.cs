using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllProductionActivityAppQuery : PagedRequest, IRequest<List<ProductionActivityApp>>
    {
        public long? companyID { get; set; }
        public GetAllProductionActivityAppQuery(long?  companyid)
        {
           this.companyID=companyid;
        }
    }
    public class GetAllNavprodOrderLine : PagedRequest, IRequest<List<NavprodOrderLineModel>>
    {
        public long? CompanyID { get; set; }
        public string? Replanrefno { get; set; }
        public GetAllNavprodOrderLine(long? companyid, string? replanrefno)
        {
            this.CompanyID = companyid;
            this.Replanrefno = replanrefno;
        }
    }
    public class GetProductActivityCaseLineTemplateItems : PagedRequest, IRequest<List<ProductActivityCaseLineModel>>
    {
        public long? ManufacturingProcessId { get; set; }
        public long? CategoryActionId { get; set; }
        public long? ProdActivityActionChildD { get; set; }
        public GetProductActivityCaseLineTemplateItems(long? manufacturingProcessId, long? categoryActionId, long? prodActivityActionChildD)
        {
            this.ManufacturingProcessId = manufacturingProcessId;
            this.CategoryActionId = categoryActionId;
            this.ProdActivityActionChildD = prodActivityActionChildD;
        }
    }
    public class GetAllProductionActivityLocationAppQuery : PagedRequest, IRequest<ProductionActivityApp>
    {
        public string? LocationName { get; set; }
        public GetAllProductionActivityLocationAppQuery( string? locationName)
        {
            this.LocationName = locationName;
        }
    }
    public class CreateProductionActivityAppCommand : ProductActivityAppModel, IRequest<long>
    {
    }
    public class GetAllProductionActivityPONumberAppQuery : PagedRequest, IRequest<List<NavprodOrderLineModel>>
    {
        public long? companyID { get; set; }
        public GetAllProductionActivityPONumberAppQuery(long? companyid)
        {
            this.companyID = companyid;
        }
    }
    public class GetAllReleaseProdOrderLineAppQuery : PagedRequest, IRequest<List<ReleaseProdOrderLine>>
    {
        public long? companyID { get; set; }
        public GetAllReleaseProdOrderLineAppQuery(long? companyid)
        {
            this.companyID = companyid;
        }
    }
    public class GetSupportingDocuments : PagedRequest, IRequest<List<DocumentsModel>>
    {
        public long? ProductionActivityPlanningAppLineID { get; set; }
        public string? Type { get; set; }
        public GetSupportingDocuments(long? productionActivityPlanningAppLineID, string? type)
        {
            this.ProductionActivityPlanningAppLineID = productionActivityPlanningAppLineID;
            this.Type = type;
        }
    }
    public class InserProductionActivityEmail : ActivityEmailTopicsModel, IRequest<ActivityEmailTopicsModel>
    {
        public ActivityEmailTopicsModel ActivityEmailTopicsModel { get; set; }
        public InserProductionActivityEmail(ActivityEmailTopicsModel activityEmailTopicsModel)
        {
            this.ActivityEmailTopicsModel = activityEmailTopicsModel;
        }
    }
    public class CreateProductionActivityRoutineAppCommand : ProductionActivityRoutineAppModel, IRequest<long>
    {
    }
   
}
