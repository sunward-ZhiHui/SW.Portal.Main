
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IEmployeeOtherDutyInformationQueryRepository : IQueryRepository<View_EmployeeOtherDutyInformation>
    {
        Task<IReadOnlyList<View_EmployeeOtherDutyInformation>> GetAllAsync(long? id);
        Task<View_EmployeeOtherDutyInformation> GetByIdAsync(Int64 id);
    }
    public interface IEmployeeICTInformationQueryRepository : IQueryRepository<View_EmployeeICTInformation>
    {
        Task<IReadOnlyList<View_EmployeeICTInformation>> GetAllAsync(long? id);
        Task<View_EmployeeICTInformation> GetByIdAsync(Int64 id);
    }
    public interface IEmployeeICTHardInformationQueryRepository : IQueryRepository<View_EmployeeICTHardInformation>
    {
        Task<IReadOnlyList<View_EmployeeICTHardInformation>> GetAllAsync(long? id);
        Task<View_EmployeeICTHardInformation> GetByIdAsync(Int64 id);
    }
}
