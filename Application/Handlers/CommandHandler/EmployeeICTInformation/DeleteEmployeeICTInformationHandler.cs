using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;


namespace Application.Handlers.CommandHandler
{
    public class DeleteEmployeeICTInformationHandler : IRequestHandler<DeleteEmployeeICTInformationCommand, String>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeICTInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeIctinformationId = queryEntity.EmployeeIctinformationId,
                    AddedByUserId = queryEntity.AddedByUserId,
                };
                var result = queryEntity;
                await _commandRepository.DeleteAsync(data);
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();
                if (result != null)
                {
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result.SoftwareId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "SoftwareId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result.SoftwareName, null, request.Id, guid, request?.UserId, DateTime.Now, true, "SoftwareName", uid);

                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result.Password, null, request.Id, guid, request?.UserId, DateTime.Now, true, "Password", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.RoleId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "RoleId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.RoleName, null, request.Id, guid, request?.UserId, DateTime.Now, true, "RoleName", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.StatusCodeId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.StatusCode, null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.ModifiedByUserId.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt"), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.ModifiedBy, null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Delete", result?.EmployeeId?.ToString(), queryEntity?.EmployeeId?.ToString(), request.Id, guid, request?.UserId, DateTime.Now, true, "EmployeeId", uid);

                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Department information has been deleted!";
        }
    }
    public class DeleteEmployeeICTHardInformationHandler : IRequestHandler<DeleteEmployeeICTHardInformationCommand, String>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public DeleteEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }

        public async Task<string> Handle(DeleteEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new EmployeeICTHardInformation
                {
                    EmployeeId = queryEntity.EmployeeId,
                    EmployeeIctHardinformationId = queryEntity.EmployeeIctHardinformationId,
                    AddedByUserId = queryEntity.AddedByUserId,
                };
                var result = queryEntity;
                await _commandRepository.DeleteAsync(data);
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();
                if (result != null)
                {
                    var date = DateTime.Now;
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result.Name?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "Name", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result.Password, null, request.Id, guid, request?.UserId, DateTime.Now, true, "Password", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.CompanyId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "CompanyId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.PlantCode, null, request.Id, guid, request?.UserId, DateTime.Now, true, "PlantCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.StatusCodeId?.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCodeId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.StatusCode, null, request.Id, guid, request?.UserId, DateTime.Now, true, "StatusCode", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.ModifiedByUserId.ToString(), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedByUserId", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt"), null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedDate", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.ModifiedBy, null, request.Id, guid, request?.UserId, DateTime.Now, true, "ModifiedBy", uid);
                    uid = Guid.NewGuid();
                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Delete", result?.EmployeeId?.ToString(), queryEntity?.EmployeeId?.ToString(), request.Id, guid, request?.UserId, DateTime.Now, true, "EmployeeId", uid);

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
