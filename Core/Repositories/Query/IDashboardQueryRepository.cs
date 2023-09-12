using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDashboardQueryRepository : IQueryRepository<EmailScheduler>
    {
        
        Task<IReadOnlyList<EmailTopics>> GetEailDashboard();
        Task<IReadOnlyList<EmailScheduler>> GetAllEmailSchedulerAsync();
        Task <IReadOnlyList<GeneralDashboard>> GetEmployeeCountAsync();
        Task<List<GenderRatio>> GetGenderRatioAsync();
    }
}
