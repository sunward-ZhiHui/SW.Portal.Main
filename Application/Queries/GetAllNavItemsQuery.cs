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
    public class GetAllNavItemsQuery : PagedRequest, IRequest<List<Navitems>>
    {
        public string SearchString { get; set; }
    }

    public class GetAllNavItemsItemSerialNoQuery : PagedRequest, IRequest<Navitems>
    {
        public string SearchString { get; set; }
        public string ItemSerialNo { get; set; }
        public GetAllNavItemsItemSerialNoQuery(string ItemSerialNo)
        {
            this.ItemSerialNo = ItemSerialNo;
        }
    }
}
