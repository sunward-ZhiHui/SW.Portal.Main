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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

            if (!string.IsNullOrEmpty(queryEntity?.LoginId))
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.LoginId?.ToString(), ColumnName = "LoginId" });
            }
            if (queryEntity?.RoleId > 0)
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.RoleId?.ToString(), ColumnName = "RoleId" });
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.RoleName?.ToString(), ColumnName = "RoleName" });

            }
            if (!string.IsNullOrEmpty(queryEntity?.Password))
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.Password, ColumnName = "Password" });
            }
            if (queryEntity?.SoftwareId > 0)
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.SoftwareId?.ToString(), ColumnName = "SoftwareId" });
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.SoftwareName?.ToString(), ColumnName = "SoftwareName" });

            }
            if (queryEntity?.StatusCodeId > 0)
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.StatusCodeId?.ToString(), ColumnName = "StatusCodeId" });
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.StatusCode?.ToString(), ColumnName = "StatusCode" });

            }
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedByUserId?.ToString(), ColumnName = "AddedByUserId" });
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedBy?.ToString(), ColumnName = "AddedBy" });
            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.EmployeeId?.ToString(), CurrentValue = queryEntity?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
            if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "EmployeeICTInformation",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = request?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            if (!string.IsNullOrEmpty(queryEntity?.LoginId))
            {
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.LoginId?.ToString()?.ToString(), ColumnName = "LoginId" });

            }
            if (queryEntity?.CompanyId > 0)
            {
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.CompanyId?.ToString()?.ToString(), ColumnName = "CompanyId" });
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.CompanyName?.ToString()?.ToString(), ColumnName = "CompanyName" });

            }
            if (!string.IsNullOrEmpty(queryEntity?.Password))
            {
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.Password?.ToString(), ColumnName = "Password" });
            }
            if (!string.IsNullOrEmpty(queryEntity?.Name))
            {
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.Name?.ToString()?.ToString(), ColumnName = "Name" });
            }
            if (!string.IsNullOrEmpty(queryEntity?.Instruction))
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.Instruction?.ToString(), ColumnName = "Instruction" });
            }
            if (queryEntity?.StatusCodeId > 0)
            {
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.StatusCodeId?.ToString()?.ToString(), ColumnName = "StatusCodeId" });
                auditList.Add(new HRMasterAuditTrail {  CurrentValue = queryEntity?.StatusCode?.ToString()?.ToString(), ColumnName = "StatusCode" });

            }
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedByUserId?.ToString(), ColumnName = "AddedByUserId" });
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
            auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.AddedBy?.ToString(), ColumnName = "AddedBy" });
            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.EmployeeId?.ToString(), CurrentValue = queryEntity?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
            if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "EmployeeICTHardInformation",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = request?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
