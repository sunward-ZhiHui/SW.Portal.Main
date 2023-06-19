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
    public class GetAllApplicationPermissionQuery : PagedRequest, IRequest<List<PortalMenuModel>>
    {
        public long? Id { get; set; }
        public GetAllApplicationPermissionQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllApplicationPermissionAllQuery : PagedRequest, IRequest<List<PortalMenuModel>>
    {
        public long? Id { get; set; }
        public GetAllApplicationPermissionAllQuery(long? Id)
        {
            this.Id = Id;
        }
    }


}
