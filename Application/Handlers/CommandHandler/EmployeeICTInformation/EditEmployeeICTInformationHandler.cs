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
    public class EditEmployeeICTInformationHandler : IRequestHandler<EditEmployeeICTInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.EmployeeIctinformationId);
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();

                if (result != null)
                {
                    bool isUpdate = false;
                    if (result.SoftwareId != queryrEntity.SoftwareId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.SoftwareId?.ToString(), queryrEntity?.SoftwareId?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "SoftwareId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.SoftwareName, queryrEntity?.SoftwareName, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "SoftwareName", uid);
                    }
                    if (result.LoginId != queryrEntity?.LoginId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.LoginId, queryrEntity?.LoginId, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "LoginId", uid);
                    }
                    if (result.Password != queryrEntity?.Password)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.Password, queryrEntity?.Password, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "Password", uid);
                    }
                    if (result.RoleId != queryrEntity?.RoleId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.RoleId?.ToString(), queryrEntity?.RoleId?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "RoleId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.RoleName, queryrEntity?.RoleName, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "RoleName", uid);
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "StatusCodeId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result.StatusCode, queryrEntity?.StatusCode, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result?.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserId", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result?.ModifiedDate != null ? result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTInformation", "Update", result?.EmployeeId?.ToString(), queryrEntity?.EmployeeId?.ToString(), request.EmployeeIctinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "EmployeeId", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
    public class EditEmployeeICTHardInformationHandler : IRequestHandler<EditEmployeeICTHardInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.EmployeeIctHardinformationId);
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTHardInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();

                if (result != null)
                {
                    bool isUpdate = false;
                    if (result.CompanyId != queryrEntity.CompanyId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.CompanyId?.ToString(), queryrEntity?.CompanyId?.ToString(), request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "CompanyId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.PlantCode, queryrEntity?.CompanyName, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "CompanyName", uid);
                    }
                    if (result.LoginId != queryrEntity?.LoginId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.LoginId, queryrEntity?.LoginId, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "LoginId", uid);
                    }
                    if (result.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.Name, queryrEntity?.Name, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Password != queryrEntity?.Password)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.Password, queryrEntity?.Password, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "Password", uid);
                    }
                    if (result.Instruction != queryrEntity?.Instruction)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.Instruction, queryrEntity?.Instruction, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "Instruction", uid);
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId?.ToString(), request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "StatusCodeId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result.StatusCode, queryrEntity?.StatusCode, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result?.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserId", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result?.ModifiedDate != null ? result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeICTHardInformation", "Update", result?.EmployeeId?.ToString(), queryrEntity?.EmployeeId?.ToString(), request.EmployeeIctHardinformationId, guid, request?.ModifiedByUserId, DateTime.Now, false, "EmployeeId", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
