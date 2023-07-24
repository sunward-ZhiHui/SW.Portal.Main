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
    public class GetAllNavItemsQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
    }

    public class GetAllNavItemsItemSerialNoNotNullQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllNavItemsByCompanyQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
        public long? CompanyId { get; set; }
        public GetAllNavItemsByCompanyQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetAllNavItemsByCompanyByItemSerialNoQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
        public long? CompanyId { get; set; }
        public GetAllNavItemsByCompanyByItemSerialNoQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetAllNavItemsItemSerialNoQuery : PagedRequest, IRequest<View_NavItems>
    {
        public string SearchString { get; set; }
        public string ItemSerialNo { get; set; }
        public GetAllNavItemsItemSerialNoQuery(string ItemSerialNo)
        {
            this.ItemSerialNo = ItemSerialNo;
        }
    }
}
