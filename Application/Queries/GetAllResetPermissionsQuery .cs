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
    public class GetResetPermissionsUserGroupUsers : PagedRequest, IRequest<List<UserGroupUser>>
    {
        public List<long?> UserIds { get; set; }
        public GetResetPermissionsUserGroupUsers(List<long?> userIds)
        {
            this.UserIds = userIds;
        }
    }


}
