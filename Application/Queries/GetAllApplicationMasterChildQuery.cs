using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllApplicationMasterChildQuery : PagedRequest, IRequest<List<ApplicationMasterChildModel>>
    {
        public string SearchString { get; set; }
        public GetAllApplicationMasterChildQuery(string SearchString)
        {
            this.SearchString = SearchString;
        }
    }
    public class GetAllApplicationMasterChildListQuery : PagedRequest, IRequest<List<ApplicationMasterChildModel>>
    {
        public string SearchString { get; set; }
        public GetAllApplicationMasterChildListQuery(string SearchString)
        {
            this.SearchString = SearchString;
        }
    }
    public class GetAllApplicationMasterChildByIdQuery : PagedRequest, IRequest<List<ApplicationMasterChildModel>>
    {
        public long? Id { get; set; }
        public GetAllApplicationMasterChildByIdQuery(long? Id)
        {
            this.Id = Id;
        }
    }

}
