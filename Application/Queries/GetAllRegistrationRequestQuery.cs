using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllRegistrationRequestQuery : PagedRequest, IRequest<List<RegistrationRequest>>
    {
    }
    public class InsertorUpdateRegistrationRequest : PagedRequest, IRequest<RegistrationRequest>
    {
        public RegistrationRequest RegistrationRequest { get; set; }
        public InsertorUpdateRegistrationRequest(RegistrationRequest registrationRequest)
        {
            this.RegistrationRequest = registrationRequest;
        }
    }
    public class DeleteRegistrationRequest : DynamicFormReport, IRequest<RegistrationRequest>
    {
        public RegistrationRequest RegistrationRequest { get; set; }
        public DeleteRegistrationRequest(RegistrationRequest registrationRequest)
        {
            this.RegistrationRequest = registrationRequest;
        }
    }
    public class GetRegistrationRequestBySession : PagedRequest, IRequest<RegistrationRequest>
    {
        public string? SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetRegistrationRequestBySession(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }


    public class GetRegistrationRequestDueDateAssignment : PagedRequest, IRequest<List<RegistrationRequestDueDateAssignment>>
    {
        public long? RegistrationRequestId { get; set; }
        public GetRegistrationRequestDueDateAssignment(long? registrationRequestId)
        {
            this.RegistrationRequestId = registrationRequestId;
        }
    }
    public class InsertorUpdateRegistrationRequestDueDateAssignment : RegistrationRequestDueDateAssignment, IRequest<RegistrationRequestDueDateAssignment>
    {
    }
    public class DeleteRegistrationRequestDueDateAssignment : DynamicFormReport, IRequest<RegistrationRequestDueDateAssignment>
    {
        public RegistrationRequestDueDateAssignment RegistrationRequestDueDateAssignment { get; set; }
        public DeleteRegistrationRequestDueDateAssignment(RegistrationRequestDueDateAssignment registrationRequestDueDateAssignment)
        {
            this.RegistrationRequestDueDateAssignment = registrationRequestDueDateAssignment;
        }
    }
    public class GetRegistrationRequestVariationForm : PagedRequest, IRequest<List<RegistrationRequestVariationForm>>
    {
        public long? RegistrationRequestId { get; set; }
        public GetRegistrationRequestVariationForm(long? registrationRequestId)
        {
            this.RegistrationRequestId = registrationRequestId;
        }
    }


    public class GetRegistrationRequestAssignmentOfJob : PagedRequest, IRequest<List<RegistrationRequestAssignmentOfJob>>
    {
        public long? RegistrationRequestId { get; set; }
        public GetRegistrationRequestAssignmentOfJob(long? registrationRequestId)
        {
            this.RegistrationRequestId = registrationRequestId;
        }
    }
    public class InsertorUpdateRegistrationRequestAssignmentOfJob : RegistrationRequestAssignmentOfJob, IRequest<RegistrationRequestAssignmentOfJob>
    {
    }
    public class DeleteRegistrationRequestAssignmentOfJob : RegistrationRequestAssignmentOfJob, IRequest<RegistrationRequestAssignmentOfJob>
    {
        public RegistrationRequestAssignmentOfJob RegistrationRequestAssignmentOfJob { get; set; }
        public DeleteRegistrationRequestAssignmentOfJob(RegistrationRequestAssignmentOfJob registrationRequestAssignmentOfJob)
        {
            this.RegistrationRequestAssignmentOfJob = registrationRequestAssignmentOfJob;
        }
    }

    public class GetRegistrationRequestProgressByRegistrationDepartment : PagedRequest, IRequest<List<RegistrationRequestProgressByRegistrationDepartment>>
    {
        public long? RegistrationRequestId { get; set; }
        public GetRegistrationRequestProgressByRegistrationDepartment(long? registrationRequestId)
        {
            this.RegistrationRequestId = registrationRequestId;
        }
    }
    public class InsertorUpdateRegistrationRequestProgressByRegistrationDepartment : RegistrationRequestProgressByRegistrationDepartment, IRequest<RegistrationRequestProgressByRegistrationDepartment>
    {
    }
    public class DeleteRegistrationRequestProgressByRegistrationDepartment : RegistrationRequestProgressByRegistrationDepartment, IRequest<RegistrationRequestProgressByRegistrationDepartment>
    {
        public RegistrationRequestProgressByRegistrationDepartment RegistrationRequestProgressByRegistrationDepartment { get; set; }
        public DeleteRegistrationRequestProgressByRegistrationDepartment(RegistrationRequestProgressByRegistrationDepartment registrationRequestProgressByRegistrationDepartment)
        {
            this.RegistrationRequestProgressByRegistrationDepartment = registrationRequestProgressByRegistrationDepartment;
        }
    }



    public class GetRegistrationRequestComittmentLetter : PagedRequest, IRequest<List<RegistrationRequestComittmentLetter>>
    {
        public long? RegistrationRequestProgressByRegistrationDepartmentId { get; set; }
        public GetRegistrationRequestComittmentLetter(long? registrationRequestId)
        {
            this.RegistrationRequestProgressByRegistrationDepartmentId = registrationRequestId;
        }
    }
    public class InsertorUpdateRegistrationRequestComittmentLetter : RegistrationRequestComittmentLetter, IRequest<RegistrationRequestComittmentLetter>
    {
    }
    public class DeleteRegistrationRequestComittmentLetter : RegistrationRequestComittmentLetter, IRequest<RegistrationRequestComittmentLetter>
    {
        public RegistrationRequestComittmentLetter RegistrationRequestComittmentLetter { get; set; }
        public DeleteRegistrationRequestComittmentLetter(RegistrationRequestComittmentLetter registrationRequestProgressByRegistrationDepartment)
        {
            this.RegistrationRequestComittmentLetter = registrationRequestProgressByRegistrationDepartment;
        }
    }




    public class GetRegistrationRequestQueries : PagedRequest, IRequest<List<RegistrationRequestQueries>>
    {
        public long? RegistrationRequestProgressByRegistrationDepartmentId { get; set; }
        public GetRegistrationRequestQueries(long? registrationRequestId)
        {
            this.RegistrationRequestProgressByRegistrationDepartmentId = registrationRequestId;
        }
    }
    public class InsertorUpdateRegistrationRequestQueries : RegistrationRequestQueries, IRequest<RegistrationRequestQueries>
    {
    }
    public class DeleteRegistrationRequestQueries : RegistrationRequestQueries, IRequest<RegistrationRequestQueries>
    {
        public RegistrationRequestQueries RegistrationRequestQueries { get; set; }
        public DeleteRegistrationRequestQueries(RegistrationRequestQueries registrationRequestQueries)
        {
            this.RegistrationRequestQueries = registrationRequestQueries;
        }
    }
}
