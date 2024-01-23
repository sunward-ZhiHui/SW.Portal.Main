
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IUserGroupQueryRepository : IQueryRepository<UserGroup>
    {
        Task<IReadOnlyList<UserGroup>> GetAllAsync();
        Task<UserGroup> InsertOrUpdateUserGroup(UserGroup userGroup);
        Task<UserGroup> DeleteUserGroup(UserGroup value);
        UserGroup GetUserGroupNameCheckValidation(string? value, long id);
    }
}
