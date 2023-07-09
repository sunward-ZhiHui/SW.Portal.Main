
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
    public interface IEmployeeQueryRepository : IQueryRepository<ViewEmployee>
    {
        Task<IReadOnlyList<ViewEmployee>> GetAllAsync();
        Task<IReadOnlyList<ViewEmployee>> GetAllUserAsync();
        Task<ViewEmployee> ResetEmployeePasswordAsync(ViewEmployee viewEmployee);
        Task<IReadOnlyList<ViewEmployee>> GetAllByStatusAsync();
        Task<ViewEmployee> GetAllBySessionAsync(Guid? SessionId);
        Task<ViewEmployee> GetAllByIdAsync(long? EmployeeId);
        Task<ViewEmployee> DeleteEmployeeReportAsync(long? EmployeeId);
        Task<IReadOnlyList<ApplicationPermission>> GetAllApplicationPermissionAsync(Int64 RoleId);

    }
    public interface IEmployeeReportTQueryoRepository : IQueryRepository<EmployeeReportTo>
    {
        Task<IReadOnlyList<EmployeeReportTo>> GetAllByIdAsync(long? EmployeeId);
    }
}
