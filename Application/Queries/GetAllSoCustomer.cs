using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSoCustomer : PagedRequest, IRequest<List<SoCustomer>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllSoCustomerByID : PagedRequest, IRequest<SoCustomer>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllSoCustomerByID(long? Id)
        {
            this.Id= Id;
        }
    }
    
}
