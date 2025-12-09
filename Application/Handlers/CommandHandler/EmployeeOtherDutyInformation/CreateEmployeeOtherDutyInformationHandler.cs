using Application.Command.Departments;
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
    public class CreateEmployeeOtherDutyInformationHandler : IRequestHandler<CreateEmployeeOtherDutyInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeOtherDutyInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(queryEntity);
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DesignationId?.ToString()?.ToString(), ColumnName = "DesignationId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DesignationName?.ToString()?.ToString(), ColumnName = "DesignationName" });
            if (queryEntity?.StartDate != null)
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.StartDate != null ? queryEntity.StartDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "StartDate" });
            }
            if (queryEntity?.EndDate != null)
            {
                auditList.Add(new HRMasterAuditTrail { PreValue = null, CurrentValue = queryEntity?.EndDate != null ? queryEntity.EndDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "EndDate" });
            }
            if (queryEntity?.DutyTypeId > 0)
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DutyTypeId?.ToString()?.ToString(), ColumnName = "DutyTypeId" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DutyTypeName?.ToString()?.ToString(), ColumnName = "DutyTypeName" });
            }
            if (queryEntity?.StatusCodeId > 0)
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.StatusCodeId?.ToString()?.ToString(), ColumnName = "StatusCodeId" });
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.StatusCode?.ToString()?.ToString(), ColumnName = "StatusCode" });
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
                    Type = "EmployeeOtherDutyInformation",
                    FormType = "Add",
                    HRMasterSetId = plantData,
                    AuditUserId = request?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)plantData,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
