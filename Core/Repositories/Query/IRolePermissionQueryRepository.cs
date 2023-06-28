using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IRolePermissionQueryRepository : IQueryRepository<RolePermission>
    {
        Task<IReadOnlyList<RolePermission>> GetAllAsync();
        Task<long> Insert(RolePermission rolepermission);
        Task<long> Update(RolePermission rolepermission);
        Task<long> Delete(long id);

    }
}
