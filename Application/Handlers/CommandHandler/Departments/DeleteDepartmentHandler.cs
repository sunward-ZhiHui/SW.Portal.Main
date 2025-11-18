using Application.Command.Departments;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentCommand, String>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _queryRepository.GetByIdAsync(request.Id);
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Department
                {
                    DepartmentId = request.Id,
                    CompanyId = queryEntity.CompanyId,
                    Name = queryEntity.DepartmentName,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId.Value,
                };

                await _commandRepository.DeleteAsync(data);
                if (result != null)
                {
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.DepartmentName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.DepartmentName, result.DepartmentName, request?.Id, guid, request?.UserId, DateTime.Now, true, "DisplayName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.DepartmentName, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DepartmentName", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.CompanyId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "CompanyId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.PlantCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "PlantCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.Description, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "Description", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.DivisionId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DivisionId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.Division, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "DivisionName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.ProfileCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ProfileCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.StatusCodeId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.StatusCode, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result.ModifiedByUserId?.ToString(), null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserID", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Delete", result?.ModifiedBy, null, request?.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);

                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Department information has been deleted!";
        }
    }
}
