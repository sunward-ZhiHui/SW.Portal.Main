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
        Task<IReadOnlyList<EmailRatio>> GetEmailRatioAsync(long id);
        Task<IReadOnlyList<EmailTopics>> GetEailDashboard();
        Task<IReadOnlyList<DynamicForm>> GetApprovalListAsync();
          Task<IReadOnlyList<EmailScheduler>> GetAllEmailSchedulerTodoAsync(long UserId);
        Task <IReadOnlyList<GeneralDashboard>> GetEmployeeCountAsync();
        Task<List<GenderRatio>> GetGenderRatioAsync();
        Task<List<Appointment>> GetAppointments(long id);
        Task<long> AddAppointmentAsync(Appointment appointment);
        Task<long> UpdateAppointmentAsync(Appointment appointment);
        Task<long> DeleteAppointmentAsync(long id);
        Task<IReadOnlyList<DynamicFormData>> GetDynamicDataAsync(long dynamicID);
        Task<IReadOnlyList<DynamicFormApproved>> GetDynamicApprovedStatusAsync(long DataFormID);
    }
}
