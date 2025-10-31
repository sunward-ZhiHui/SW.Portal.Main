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
    public class EditEmployeeOtherDutyInformationHandler : IRequestHandler<EditEmployeeOtherDutyInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        private readonly IEmployeeOtherDutyInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository customerRepository, IEmployeeOtherDutyInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeOtherDutyInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                var result = await _queryRepository.GetByIdAsync(queryrEntity.EmployeeOtherDutyInformationId);
                await _commandRepository.UpdateAsync(queryrEntity);
                var guid = Guid.NewGuid();
                var uid = Guid.NewGuid();

                if (result != null)
                {
                    bool isUpdate = false;
                    if (result.DesignationId != queryrEntity.DesignationId)
                    {
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result.DesignationId?.ToString(), queryrEntity?.DesignationId?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DesignationId", uid);
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result.DesignationName, queryrEntity?.DesignationName, request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DesignationName", uid);
                    }
                    if (queryrEntity?.StartDate != result.StartDate)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.StartDate != null ? result.StartDate.Value.ToString("dd-MMM-yyyy") : null, queryrEntity?.StartDate != null ? queryrEntity.StartDate.Value.ToString("dd-MMM-yyyy") : null, request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StartDate", uid);
                    }
                    if (queryrEntity?.EndDate != result?.EndDate)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.EndDate != null ? result.EndDate.Value.ToString("dd-MMM-yyyy") : null, queryrEntity?.EndDate != null ? queryrEntity.EndDate.Value.ToString("dd-MMM-yyyy") : null, request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "EndDate", uid);
                    }
                    if (queryrEntity?.DutyTypeId != result?.DutyTypeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.DutyTypeId?.ToString(), queryrEntity?.DutyTypeId?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DutyTypeId", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.DutyType, queryrEntity?.DutyTypeName?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DutyTypeName", uid);

                    }
                    if (queryrEntity?.StatusCodeId != result?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.StatusCode, queryrEntity?.StatusCode, request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "AddedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.ModifiedDate != null ? result?.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("EmployeeOtherDutyInformation", "Update", result?.EmployeeId?.ToString(), queryrEntity?.EmployeeId?.ToString(), request.EmployeeOtherDutyInformationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "EmployeeId", uid);
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
