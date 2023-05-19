
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
    public interface IAppPermissionQueryRepository : IQueryRepository<AppPermissionModel>
    {
        Task<IReadOnlyList<AppPermissionModel>> GetAllAsync();
        Task<AppPermissionModel> GetByIdAsync(Int64 id);
        List<AppPermissionModel> GetAllByAsync();
    }

    
}
