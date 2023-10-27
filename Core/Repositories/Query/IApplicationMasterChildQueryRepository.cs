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
    public interface IApplicationMasterChildQueryRepository : IQueryRepository<ApplicationMasterChildModel>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByAsync(string Ids);
        Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllAsync(string Ids);
        Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByIdAsync(long? Id);
        Task<ApplicationMasterChildModel> GetAllByChildIDAsync(long? Id);
        Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByProAsync();
    }
}
