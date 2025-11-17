using Application.Command.designations;
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
    public class EditDesignationHandler : IRequestHandler<EditDesignationCommand, DesignationResponse>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDesignationHandler(IDesignationCommandRepository customerRepository, IDesignationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DesignationResponse> Handle(EditDesignationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DesignationId);
            var queryrEntity = RoleMapper.Mapper.Map<Designation>(request);

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
                    if (result.Code != queryrEntity.Code)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.Code, queryrEntity?.Code, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Code", uid);
                    }
                    if (result.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.Name, queryrEntity?.Name, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.Description, queryrEntity?.Description, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Description", uid);
                    }
                    if (result.CompanyID != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.CompanyID?.ToString(), queryrEntity?.CompanyId.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.CompanyName, queryrEntity?.CompanyName, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "CompanyName", uid);
                    }
                    if (result.SectionID != queryrEntity?.SectionId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.SectionID?.ToString(), queryrEntity?.SectionId.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SectionId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.SectionName, queryrEntity?.SectionName, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SectionName", uid);
                    }
                    if (result.SubSectionID != queryrEntity?.SubSectionId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.SubSectionID?.ToString(), queryrEntity?.SubSectionId.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SubSectionId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.SubSectionName, queryrEntity?.SubSectionName, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SubSectionName", uid);
                    }
                    if (result.LevelID != queryrEntity?.LevelId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.LevelID?.ToString(), queryrEntity?.LevelId.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "LevelID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.LevelName, queryrEntity?.LevelName, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "LevelName", uid);
                    }
                    if (result.HeadCount != queryrEntity?.HeadCount)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.HeadCount?.ToString(), queryrEntity?.HeadCount?.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "HeadCount", uid);
                    }
                    if (result.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.StatusCodeID?.ToString(), queryrEntity?.StatusCodeId.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", queryrEntity?.Name, queryrEntity?.Name, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result.ModifiedByUserID?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.DesignationId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new DesignationResponse
            {
                DesignationID = queryrEntity.DesignationId,
                CompanyID = queryrEntity.CompanyId.Value,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
                Name =queryrEntity.Name,
            };

            return response;
        }
    }
}
