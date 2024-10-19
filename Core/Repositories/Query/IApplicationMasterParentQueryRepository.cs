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
    public interface IApplicationMasterParentQueryRepository : IQueryRepository<ApplicationMasterParent>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ApplicationMasterParent>> GetAllAsync();
        Task<IReadOnlyList<ApplicationMasterParent>> GetAllByParentAsync();
        Task<ApplicationMasterParent> InsertApplicationMasterParent(ApplicationMasterParent value);
        Task<IReadOnlyList<ApplicationMasterChildModel>> GetAllByNested(long? ApplicationMasterParentCodeId);
    }
}
