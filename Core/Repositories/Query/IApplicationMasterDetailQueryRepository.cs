using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IApplicationMasterDetailQueryRepository : IQueryRepository<View_ApplicationMasterDetail>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<View_ApplicationMasterDetail>> GetApplicationMasterByCode(long? Id);
        Task<View_ApplicationMasterDetail> GetByIdAsync(long? Id);
        Task<IReadOnlyList<ApplicationMasterDetail>> GetAllAsync();
        Task<ApplicationMaster> InsertApplicationMaster(ApplicationMaster value);
        Task<ApplicationMasterAccess> InsertApplicationMasterAccessSecurity(ApplicationMasterAccess value);
        Task<IReadOnlyList<ApplicationMasterAccess>> GetApplicationMasterAccessSecurityList(long? Id,string? AccessTypeFrom);
        Task<long> DeleteApplicationMasterAccess(long? Id, List<long?> Ids);
        Task<IReadOnlyList<ApplicationMasterAccess>> GetApplicationMasterAccessSecurityEmptyAsync(long? ApplicationMasterId, string? AccessTypeFrom);
    }
}
