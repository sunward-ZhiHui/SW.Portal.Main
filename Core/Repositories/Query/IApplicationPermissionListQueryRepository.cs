using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IApplicationPermissionListQueryRepository : IQueryRepository<ApplicationPermission>
    {
        Task<IReadOnlyList<ApplicationPermission>> GetAllAsync(long parentId);
        Task<long> Insert(ApplicationPermission applicationPermission);
        Task<long> Update(ApplicationPermission applicationPermission);
        Task<long> UpdateOrder(ApplicationPermission applicationPermission);        
        Task<long> Delete(long id, long permissionid );
        Task<IReadOnlyList<ApplicationPermission>> GetAllListByParentIDAsync(string parentID);
        Task<IReadOnlyList<ApplicationPermission>> GetAllListByParentAsync();
        Task<IReadOnlyList<ApplicationPermission>> GetAllListBySessionIDAsync(Guid? SessionID);
        Task<long> InsertPermission(ApplicationPermission applicationPermission);
        
    }
}
