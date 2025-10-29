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

namespace Application.Handlers.CommandHandler.Designations
{
    public class CreateDesignationHandler : IRequestHandler<CreateDesignationCommand, DesignationResponse>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateDesignationHandler(IDesignationCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DesignationResponse> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Designation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.Code, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Code", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", queryEntity?.Name, null, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DisplayName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.Name, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Name", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.Description, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Description", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.CompanyId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.CompanyName?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.SectionName, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "SectionName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.SectionId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "SectionId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.SubSectionId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "SubSectionID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.SubSectionName, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "SubSectionName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.LevelId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "LevelID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.LevelName?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "LevelName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.HeadCount?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "HeadCount", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.StatusCodeId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCodeID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.StatusCode, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.ModifiedByUserId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "ModifiedByUserID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.ModifiedDate != null ? queryEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "ModifiedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("Designation", "Add", null, queryEntity?.ModifiedBy, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "ModifiedBy", uid);


            var response = new DesignationResponse
            {
                DesignationID = (long)data,
                StatusCodeID = queryEntity.StatusCodeId,
                Name = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
