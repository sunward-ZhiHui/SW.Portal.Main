using Application.Command.LeveMasters;
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

namespace Application.Handlers.CommandHandler.LevelMasters
{
    public class CreateLevelMasterHandler : IRequestHandler<CreateLevelMasterCommand, LevelMasterResponse>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public CreateLevelMasterHandler(ILevelMasterCommandRepository commandRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = commandRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<LevelMasterResponse> Handle(CreateLevelMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<LevelMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var guid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", queryEntity?.Name, queryEntity?.Name, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DisplayName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.Name, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "Name", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.CompanyId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.CompanyName?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "CompanyName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.DivisionID?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DivisionID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.DivisionName, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "DivisionName", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.StatusCodeId.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCodeID", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.StatusCode, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "StatusCode", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.AddedByUserId?.ToString(), data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedByUserId", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedDate", uid);
            uid = Guid.NewGuid();
            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("LevelMaster", "Add", null, queryEntity?.AddedBy, data, guid, queryEntity?.AddedByUserId, DateTime.Now, false, "AddedBy", uid);

            var response = new LevelMasterResponse
            {
                LevelID = queryEntity.LevelId,
                StatusCodeID = queryEntity.StatusCodeId,
                Name = queryEntity.Name,
            };
            return response;
        }
    }
}
