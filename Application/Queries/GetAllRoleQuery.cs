using Application.Queries.Base;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllRoleQuery : PagedRequest, IRequest<List<ApplicationRole>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllUserGroupsQuery : PagedRequest, IRequest<List<UserGroup>>
    {
        public string SearchString { get; set; }
    }
    public class InsertOrUpdateUserGroup : UserGroup, IRequest<UserGroup>
    {

    }
    public class DeleteUserGroup : UserGroup, IRequest<UserGroup>
    {
        public UserGroup UserGroup { get; private set; }
        public DeleteUserGroup(UserGroup userGroup)
        {
            this.UserGroup = userGroup;
        }
    }
}
