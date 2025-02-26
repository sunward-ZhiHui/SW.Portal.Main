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
    public interface IRegistrationRequestQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<RegistrationRequest>> GetRegistrationRequest();
        Task<RegistrationRequest> DeleteRegistrationRequest(RegistrationRequest value);
        Task<RegistrationRequest> InsertorUpdateRegistrationRequest(RegistrationRequest value);
        Task<RegistrationRequest> GetRegistrationRequestBySession(Guid? sessionId);
        Task<IReadOnlyList<RegistrationRequestDueDateAssignment>> GetRegistrationRequestDueDateAssignment(long? RegistrationRequestId);
        Task<RegistrationRequestDueDateAssignment> InsertorUpdateRegistrationRequestDueDateAssignment(RegistrationRequestDueDateAssignment value);
        Task<RegistrationRequestDueDateAssignment> DeleteRegistrationRequestDueDateAssignment(RegistrationRequestDueDateAssignment value);
        Task<IReadOnlyList<RegistrationRequestVariationForm>> GetRegistrationRequestVariationForm(long? RegistrationRequestId);
    }
}
