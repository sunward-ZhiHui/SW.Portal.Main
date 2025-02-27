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
        Task<IReadOnlyList<Appointment>> GetSchedulerListAsync(long Userid);
        Task<string> GetPendingRemindersAsync();        
        Task<int> GetSchedulerCountAsync(long Userid);
        Task<int> GetInvitationCountAsync(long Userid);
        Task<int> GetUserCreatedSchedulerCountAsync(long Userid);
        Task<IReadOnlyList<Appointment>> GetUserListAsync(long Appointmentid);
        Task<IReadOnlyList<Appointment>> GetEmailListAsync(long Appointmentid);
        Task<IReadOnlyList<Appointment>> GetUserListNotificationAsync(long ID);
        Task<IReadOnlyList<Appointment>> GetNotificationCaptionAsync(long ID);
        Task<List<GenderRatio>> GetGenderRatioAsync();
        Task<List<Appointment>> GetAppointments(long id);
        Task<long> AddAppointmentAsync(Appointment appointment);
        Task<long> AddAppointmentinsertAsync(Appointment userMultiple);
        Task<long> AddAppointmentEmailinsertAsync(Appointment userMultiple);
        Task<long> UpdateAcceptAsync(long userid,bool accept,long appointmentid);
           Task<long> UpdateAppointmentAsync(Appointment appointment);
        Task<long> DeleteAppointmentAsync(long id);
        Task<long> DeleteUsermultipleAsync(long appointmentid);
        Task<long> DeleteEmailmultipleAsync(long appointmentid);
        Task<IReadOnlyList<DynamicFormData>> GetDynamicDataAsync(long dynamicID);
        Task<IReadOnlyList<DynamicFormApproved>> GetDynamicApprovedStatusAsync(long DataFormID);
        Task<List<Appointment>> GetCreatedUserAsync(long appointmentid,long userid);
        Task<List<Appointment>> GetAcceptedUserAsync(long appointmentid, long userid);
        
    }
}
