using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IRolePermissionQueryRepository : IQueryRepository<ApplicationRole>
    {
        Task<IReadOnlyList<ApplicationRole>> GetAllAsync();
        Task<long> Insert(ApplicationRole rolepermission);
        Task<long> Update(ApplicationRole rolepermission);
        Task<long> Delete(long id);
        Task<List<ApplicationPermission>> GetSelectedRolePermissionListAsync(Int64 RoleId);

    }
}
