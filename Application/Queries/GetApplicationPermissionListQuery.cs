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
    public class GetApplicationPermissionListQuery : PagedRequest, IRequest<List<ApplicationPermission>>
    {
    }

    public class CreateApplicationPermissionListQuery : ApplicationPermission, IRequest<long>
    {
    }
    public class EditApplicationPermissionListQuery : ApplicationPermission, IRequest<long>
    {

    }
    public class DeleteApplicationPermissionListQuery : ApplicationPermission, IRequest<long>
    {
        public long ID { get; set; }
        public long permissionid { get; set; }

        public DeleteApplicationPermissionListQuery(long Id, long permissionid)
        {
            this.ID = Id;

            this.permissionid = permissionid;
        }
    }
   
}
