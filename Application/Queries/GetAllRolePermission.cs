using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllRolePermission : PagedRequest, IRequest<List<RolePermission>>
    {
   
    }
    public class CreateRolePermissionQuery : RolePermission, IRequest<long>
    {
    }
    public class EditRolePermissionQuery : RolePermission, IRequest<long>
    {
   
    }
    public class DeleteRolePermissionQuery : RolePermission, IRequest<long>
    {
        public long ID { get; set; }

        public DeleteRolePermissionQuery(long Id)
        {
            this.ID = Id;
        }
    }
}
