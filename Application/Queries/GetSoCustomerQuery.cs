using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetSoCustomerQuery : PagedRequest, IRequest<List<SoCustomer>>
    {
        public string SearchString { get; set; }
    }


    public class GetSalesOrderLine : PagedRequest, IRequest<List<View_SoSalesOrderLine>>
    {
        public string SearchString { get; set; }
    }

    public class GetPostSalesOrder : PagedRequest, IRequest<List<PostSalesOrder>>
    {
        public string SearchString { get; set; }
    }
}
