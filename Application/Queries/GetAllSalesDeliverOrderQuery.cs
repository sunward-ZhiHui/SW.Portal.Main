using Application.Queries.Base;
using CMS.Application.Handlers.QueryHandlers;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSalesDeliverOrderQuery : PagedRequest, IRequest<List<NavpostedShipment>>
    {
        public NavpostedShipment NavpostedShipment { get; set; }
        public string? SearchString { get; set; }
        public GetAllSalesDeliverOrderQuery(NavpostedShipment shipment)
        {
            this.NavpostedShipment = shipment;
        }
    }
    public class GetSyncSalesDeliverOrderQuery : PagedRequest, IRequest<NavpostedShipment>
    {
        public NavpostedShipment NavpostedShipment { get; set; }
        public GetSyncSalesDeliverOrderQuery(NavpostedShipment shipment)
        {
            this.NavpostedShipment = shipment;
        }
    }
    public class InsertOrUpdateSalesDeliverOrder : NavpostedShipment, IRequest<NavpostedShipment>
    {

    }
}
