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

namespace Application.Handlers.CommandHandler.Sections
{
    public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, SectionResponse>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateSectionHandler(ISectionCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<SectionResponse> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Section>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", queryEntity?.Name, queryEntity?.Name, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DisplayName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.Name, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Name", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.Code, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Code", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.Description, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Description", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.DepartmentId.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DepartmentId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.DepartmentName, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DepartmentName", uid);
            if (!string.IsNullOrEmpty(queryEntity?.ProfileCode))
            {
                uid = Guid.NewGuid();
                await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.ProfileCode, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "ProfileCode", uid);
            }
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.StatusCodeId.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCodeID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.StatusCode, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.AddedByUserId?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Section", "Add", null, queryEntity?.AddedBy?.ToString(), plantData, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);

            var response = new SectionResponse
            {
                SectionId = (long)plantData,
                StatusCodeId = queryEntity.StatusCodeId,
                SectionName = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
