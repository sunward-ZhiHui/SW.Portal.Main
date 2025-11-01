using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateEmployeeICTInformationHandler : IRequestHandler<CreateEmployeeICTInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeICTInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            if (!string.IsNullOrEmpty(queryEntity?.LoginId))
            {
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.LoginId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "LoginId", uid);

            }
            if (queryEntity?.RoleId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.RoleId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "RoleId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.RoleName?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "RoleName", uid);

            }
            if (!string.IsNullOrEmpty(queryEntity?.Password))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.Password, data, guid, request?.AddedByUserId, DateTime.Now, false, "Password", uid);
            }
            if (queryEntity?.SoftwareId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.SoftwareId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "SoftwareId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.SoftwareName?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "SoftwareName", uid);

            }
            if (queryEntity?.StatusCodeId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.StatusCodeId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "StatusCodeId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.StatusCode?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);

            }
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.AddedByUserId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedByUserId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", null, queryEntity?.AddedBy?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Add", queryEntity?.EmployeeId?.ToString(), queryEntity?.EmployeeId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "EmployeeId", uid);

            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
    public class CreateEmployeeICTHardInformationHandler : IRequestHandler<CreateEmployeeICTHardInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeICTHardInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            if (!string.IsNullOrEmpty(queryEntity?.LoginId))
            {
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.LoginId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "LoginId", uid);

            }
            if (queryEntity?.CompanyId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.CompanyId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "CompanyId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.CompanyName?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "CompanyName", uid);

            }
            if (!string.IsNullOrEmpty(queryEntity?.Password))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.Password, data, guid, request?.AddedByUserId, DateTime.Now, false, "Password", uid);
            }
            if (!string.IsNullOrEmpty(queryEntity?.Name))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.Name?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "Name", uid);
            }
            if (!string.IsNullOrEmpty(queryEntity?.Instruction))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.Instruction, data, guid, request?.AddedByUserId, DateTime.Now, false, "Instruction", uid);
            }
            if (queryEntity?.StatusCodeId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.StatusCodeId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "StatusCodeId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.StatusCode?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);

            }
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.AddedByUserId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedByUserId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", null, queryEntity?.AddedBy?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Add", queryEntity?.EmployeeId?.ToString(), queryEntity?.EmployeeId?.ToString(), data, guid, request?.AddedByUserId, DateTime.Now, false, "EmployeeId", uid);

            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
