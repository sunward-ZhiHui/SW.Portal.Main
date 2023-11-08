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
        public GetProductActivityCaseLineTemplateItems(long? manufacturingProcessId, long? categoryActionId)
        {
            this.ManufacturingProcessId = manufacturingProcessId;
            this.CategoryActionId = categoryActionId;
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

}
