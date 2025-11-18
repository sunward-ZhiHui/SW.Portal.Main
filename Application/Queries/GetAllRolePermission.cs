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
    public class GetAllRolePermission : PagedRequest, IRequest<List<ApplicationRole>>
    {
   
    }
    public class CreateRolePermissionQuery : ApplicationRole, IRequest<long>
    {
    }
    public class EditRolePermissionQuery : ApplicationRole, IRequest<long>
    {
   
    }
    public class DeleteRolePermissionQuery : ApplicationRole, IRequest<long>
    {
        public long ID { get; set; }
        public long? UserId { get; set; }

        public DeleteRolePermissionQuery(long Id, long? userId)
        {
            this.ID = Id;
            UserId = userId;
        }
    }
    public class GetAllRolePermissionSelectedLst : PagedRequest, IRequest<List<ApplicationPermission>>
    {
        public long RoleID { get; private set; }
        public GetAllRolePermissionSelectedLst(long RoleId)
        {
            this.RoleID = RoleId;
        }
    }
}
