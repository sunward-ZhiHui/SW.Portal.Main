using Application.Command.Departments;
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
    public class EditDepartmentHandler : IRequestHandler<EditDepartmentCommand, DepartmentResponse>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DepartmentResponse> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DepartmentId);
            var queryrEntity = RoleMapper.Mapper.Map<Department>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
                if (result != null)
                {
                    bool isUpdate = false;
                    var guid = Guid.NewGuid();
                    var uid = Guid.NewGuid();
                    if (result.DepartmentCode != queryrEntity.Code)
                    {
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.DepartmentCode, queryrEntity?.Code, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Code", uid);
                    }
                    if (result.DepartmentName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.DepartmentName, queryrEntity?.Name, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.Description, queryrEntity?.Description, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Description", uid);
                    }
                    if (result.CompanyId != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.CompanyId?.ToString(), queryrEntity?.CompanyId?.ToString(), queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.PlantCode, queryrEntity?.CompanyName, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyName", uid);
                    }
                    if (result.DivisionId != queryrEntity?.DivisionId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.DivisionId?.ToString(), queryrEntity?.DepartmentId.ToString(), queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DivisionId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.Division, queryrEntity?.DivisionName, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DivisionName", uid);
                    }
                    if (result.ProfileCode != queryrEntity?.ProfileCode)
                    {
                        uid = Guid.NewGuid();
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.ProfileCode, queryrEntity?.ProfileCode, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ProfileCode", uid);
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId.ToString(), queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", queryrEntity?.Name, null, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Department", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.DepartmentId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new DepartmentResponse
            {
                DepartmentId = queryrEntity.DepartmentId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                DepartmentName = queryrEntity.Name,
            };

            return response;
        }
    }
}
