using Application.Command.Departments;
using Application.Command.SubSections;
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
    public class EditSubSectionHandler : IRequestHandler<EditSubSectionCommand, SubSectionResponse>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        private readonly ISubSectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditSubSectionHandler(ISubSectionCommandRepository customerRepository, ISubSectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<SubSectionResponse> Handle(EditSubSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.SubSectionId);
            var queryrEntity = RoleMapper.Mapper.Map<SubSection>(request);

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
                    if (result.SubSectionCode != queryrEntity.Code)
                    {
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.SubSectionCode, queryrEntity.Code, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Code", uid);
                    }
                    if (result.SubSectionName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.SubSectionName, queryrEntity.Name, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Name", uid);
                    }
                    if (result.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.Description, queryrEntity.Description, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "Description", uid);
                    }
                    if (result.SectionId != queryrEntity?.SectionId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.SectionId?.ToString(), queryrEntity?.SubSectionId.ToString(), queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SectionId", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.SectionName, queryrEntity?.SectionName, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "SectionName", uid);
                    }
                    if (result.ProfileCode != queryrEntity?.ProfileCode)
                    {
                        uid = Guid.NewGuid();
                        isUpdate = true;
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.ProfileCode, queryrEntity?.ProfileCode, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ProfileCode", uid);
                    }
                    if (result.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.StatusCodeId?.ToString(), queryrEntity?.StatusCodeId.ToString(), queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCodeID", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.StatusCode, queryrEntity?.StatusCode, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "StatusCode", uid);
                    }
                    if (isUpdate)
                    {
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", queryrEntity?.Name,null, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "DisplayName", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result.ModifiedByUserId?.ToString(), queryrEntity?.ModifiedByUserId?.ToString(), queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result?.ModifiedDate != null ? result.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("SubSection", "Update", result?.ModifiedBy, queryrEntity?.ModifiedBy?.ToString(), queryrEntity?.SubSectionId, guid, queryrEntity?.ModifiedByUserId, DateTime.Now, false, "ModifiedBy", uid);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SubSectionResponse
            {
                SubSectionId = queryrEntity.SubSectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SubSectionName =queryrEntity.Name,
            };

            return response;
        }
    }
}
