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
    public class GetAllApplicationMasterParentQuery : PagedRequest, IRequest<List<ApplicationMasterParent>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllApplicationMasterParentAllQuery : PagedRequest, IRequest<List<ApplicationMasterParent>>
    {
        public string SearchString { get; set; }
    }
    

}
