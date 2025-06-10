using Application.Queries.Base;
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
    public class GetAllSalesOrderQuery : PagedRequest, IRequest<List<SalesOrderModel>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateSalesOrder : SalesOrderModel, IRequest<SalesOrderModel>
    {

    }
    public class DeleteSalesOrder : SalesOrderModel, IRequest<SalesOrderModel>
    {
        public SalesOrderModel SalesOrderModel { get; set; }
        public DeleteSalesOrder(SalesOrderModel salesOrderModel)
        {
            this.SalesOrderModel = salesOrderModel;
        }
    }
}
