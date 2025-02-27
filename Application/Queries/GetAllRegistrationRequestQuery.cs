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
    
}
