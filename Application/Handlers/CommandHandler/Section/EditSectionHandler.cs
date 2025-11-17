using Application.Command.Departments;
using Application.Command.Sections;
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
    public class EditSectionHandler : IRequestHandler<EditSectionCommand, SectionResponse>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly ISectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;

        public EditSectionHandler(ISectionCommandRepository customerRepository, ISectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<SectionResponse> Handle(EditSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.SectionId);
            var queryrEntity = RoleMapper.Mapper.Map<Section>(request);

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
                    if (result.SectionCode != queryrEntity.Code)
                    {
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.SectionCode, queryrEntity?.Code, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Code", uid);
                    }
                    if (result.SectionName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.SectionName, queryrEntity?.Name, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.Description, queryrEntity?.Description, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Description", uid);
                    }
                    if (result.DepartmentId != queryrEntity?.DepartmentId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.DepartmentId?.ToString(), queryrEntity?.DepartmentId.ToString(), queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DepartmentId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.DepartmentName, queryrEntity?.DepartmentName, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DepartmentName", uid);
                    }
                    if (result.ProfileCode != queryrEntity?.ProfileCode)
                    {
                        uid = Guid.NewGuid();
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.ProfileCode, queryrEntity?.ProfileCode, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ProfileCode", uid);
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId.ToString(), queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", queryrEntity?.Name, queryrEntity?.Name, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.SectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SectionResponse
            {
                SectionId = queryrEntity.SectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SectionName = queryrEntity.Name,
            };

            return response;
        }
    }
}
