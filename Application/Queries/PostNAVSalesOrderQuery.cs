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
    public class PostNAVSalesOrderQuery : PostSalesOrder, IRequest<PostSalesOrder>
    {
        public string SearchString { get; set; }
    }  
    public class rawitemSalesOrderQuery : PagedRequest, IRequest<string>
    {
        public string CompanyName { get; set; }
        public long? Companyid { get; set; }
        public string Type { get; set; }
        public rawitemSalesOrderQuery(string CompanyName,long? Companyid,string type)
        {
            this.CompanyName = CompanyName;
            this.Companyid = Companyid;
            this.Type = type;
        }
    }
   
}
