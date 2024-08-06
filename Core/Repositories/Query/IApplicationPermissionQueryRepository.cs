using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public  interface IApplicationPermissionQueryRepository:IQueryRepository<ApplicationPermission>
    {
       
        Task<IReadOnlyList<ApplicationPermission>> GetAllAsync();
        Task<long> Insert(ApplicationRolePermission applicationrolepermission);
        Task<List<ApplicationPermission>> GetByListSessionIdAsync(string sessionid);
    }
}
