using Application.Command.Departments;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeOtherDutyInformationHandler : IRequestHandler<DeleteEmployeeOtherDutyInformationCommand, String>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        private readonly IEmployeeOtherDutyInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository customerRepository, IEmployeeOtherDutyInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeOtherDutyInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeOtherDutyInformationId = request.Id,
                    AddedByUserId = queryEntity.AddedByUserId,
                };
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();

                if (result != null)
                {
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result.DesignationId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "DesignationId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result.DesignationName, null, request.Id, guid, request?.UserId, DateTime.Now, true, "DesignationName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.StartDate != null ? result.StartDate.Value.ToString("dd-MMM-yyyy") : null, null, request.Id, guid, request?.UserId, DateTime.Now, true, "StartDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.EndDate != null ? result.EndDate.Value.ToString("dd-MMM-yyyy") : null, null, request.Id, guid, request?.UserId, DateTime.Now, true, "EndDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.DutyTypeId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "DutyTypeId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.DutyType, null, request.Id, guid, request?.UserId, DateTime.Now, true, "DutyTypeName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.StatusCodeId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.StatusCode, null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.ModifiedByUserId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.ModifiedDate != null ? result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.ModifiedBy, null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Delete", result?.EmployeeId?.ToString(), result?.EmployeeId?.ToString(), request.Id, guid, request?.UserId, DateTime.Now, true, "EmployeeId", uid);

                }
                await _commandRepository.DeleteAsync(data);

            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Department information has been deleted!";
        }
    }
}
