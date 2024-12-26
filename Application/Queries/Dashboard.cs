using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class Dashboard : IRequest<List<EmailScheduler>>
    {
        public string SearchString { get; set; }
    }
   

    public class GetEmployeeCount : PagedRequest, IRequest<List<GeneralDashboard>>
    {
       
    }
    public class GetUserListQuery : PagedRequest, IRequest<List<Appointment>>
    {
        public long AppointmentID { get; private set; }
        public GetUserListQuery(long AppointmentID)
        {
            this.AppointmentID = AppointmentID;
        }
    }
    public class GetUserListSchedulerNotificationQuery : PagedRequest, IRequest<List<Appointment>>
    {
        public long ID { get; private set; }
        public GetUserListSchedulerNotificationQuery(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetSchedulerNotificationCaptionQuery : PagedRequest, IRequest<List<Appointment>>
    {
        public long ID { get; private set; }
        public GetSchedulerNotificationCaptionQuery(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetSchedulerList: PagedRequest, IRequest<List<Appointment>>
    {
        public long UserID { get; private set; }
        public GetSchedulerList(long UserID)
        {
            this.UserID = UserID;
        }
    }
    public class GetSchedulerCout : PagedRequest, IRequest<int>
    {
        public long UserID { get; private set; }
        public GetSchedulerCout(long UserID)
        {
            this.UserID = UserID;
        }
    }    
    public class GetGenderRatio : PagedRequest, IRequest<List<GenderRatio>>
    {

    }
    public class GetEmailDasboard : IRequest<List<EmailTopics>>
    {
        public string SearchString { get; set; }
    }
    public class GetEmailRatio : PagedRequest, IRequest<List<EmailRatio>>
    {
        public long UserId { get; private set; }
        public GetEmailRatio(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetEmailSchedulerListTodo : PagedRequest, IRequest<List<EmailScheduler>>
    {
        public long UserId { get; private set; }
        public GetEmailSchedulerListTodo(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetAppointmentList : PagedRequest, IRequest<List<Appointment>>
    {
        public long UserId { get; private set; }
        public GetAppointmentList(long UserId)
        {
            this.UserId = UserId;
        }
    }
    
    public class AddAppointment : Appointment, IRequest<long>
    {
    }
    public class EditAppointment : Appointment, IRequest<long>
    {
    }
    public class UpdateAccept : Appointment, IRequest<long>
    {
        public long userID { get; private set; }
        public bool Accept { get; private set; }
        public long Appointmentid { get; private set; }
        public UpdateAccept(long userID, bool accept, long appointmentid)
        {
            this.userID = userID;
            Accept = accept;
            Appointmentid = appointmentid;
        }
    }
    public class DeleteAppointment : IRequest<long>
    {
        public long Id { get; private set; }
        public DeleteAppointment(long Id)
        {
            this.Id = Id;
        }
    }
    public class DynamicApprovalListOne : IRequest<List<DynamicForm>>
    {
       
    }
    public class DynamicApprovalListtwo: IRequest<List<DynamicFormData>>
    {
        public long DynamicId { get; private set; }
        public DynamicApprovalListtwo(long DynamicId)
        {
            this.DynamicId = DynamicId;
        }
    }
    public class DynamicApprovalListthired : IRequest<List<DynamicFormApproved>>
    {
        public long FormDataId { get; private set; }
        public DynamicApprovalListthired(long FormDataId)
        {
            this.FormDataId = FormDataId;
        }
    }
    public class GetCreatedUserQuery : PagedRequest, IRequest<List<Appointment>>
    {
        public long AppointmentID { get; private set; }
        public long userid { get; private set; }
        public GetCreatedUserQuery(long AppointmentID, long userid)
        {
            this.AppointmentID = AppointmentID;
            this.userid = userid;
        }
    }
    public class GetUserQuery : PagedRequest, IRequest<List<Appointment>>
    {
        public long AppointmentID { get; private set; }
        public long userid { get; private set; }
        public GetUserQuery(long AppointmentID, long userid)
        {
            this.AppointmentID = AppointmentID;
            this.userid = userid;
        }
    }
}
