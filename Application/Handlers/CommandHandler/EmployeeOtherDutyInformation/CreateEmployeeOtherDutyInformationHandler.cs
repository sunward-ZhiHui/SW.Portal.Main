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
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();

            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.DesignationId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DesignationId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.DesignationName, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DesignationName", uid);
            if (queryEntity?.StartDate != null)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.StartDate != null ? queryEntity.StartDate.Value.ToString("dd-MMM-yyyy") : null, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StartDate", uid);
            }
            if (queryEntity?.EndDate != null)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.EndDate != null ? queryEntity.EndDate.Value.ToString("dd-MMM-yyyy") : null, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "EndDate", uid);
            }
            if (queryEntity?.DutyTypeId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.DutyTypeId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DutyTypeId", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.DutyTypeName?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DutyTypeName", uid);

            }
            if (queryEntity?.StatusCodeId > 0)
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.StatusCodeId.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCodeID", uid);
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.StatusCode, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);
            }
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.AddedByUserId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", null, queryEntity?.AddedBy?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Add", queryEntity?.EmployeeId?.ToString(), queryEntity?.EmployeeId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "EmployeeId", uid);

            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)plantData,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
