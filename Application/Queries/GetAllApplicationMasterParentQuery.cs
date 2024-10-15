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
    public class InsertApplicationMasterParent : ApplicationMasterParent, IRequest<ApplicationMasterParent>
    {

    }
    public class GetAllApplicationMasterChildNestedQuery : PagedRequest, IRequest<List<ApplicationMasterChildModel>>
    {
        public long? ApplicationMasterParentCodeId { get; set; }
        public GetAllApplicationMasterChildNestedQuery(long? applicationMasterParentCodeId)
        {
            this.ApplicationMasterParentCodeId = applicationMasterParentCodeId;
        }
    }
}
