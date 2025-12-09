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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Name, ColumnName = "Name" });
            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.Name, CurrentValue = queryEntity?.Name, ColumnName = "DisplayName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.CompanyName, ColumnName = "CompanyName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DivisionID?.ToString(), ColumnName = "DivisionID" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DivisionName, ColumnName = "DivisionName" });

            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.StatusCodeId.ToString(), ColumnName = "StatusCodeId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.StatusCode, ColumnName = "StatusCode" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.AddedByUserId?.ToString(), ColumnName = "AddedByUserId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.AddedDate != null ? queryEntity.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.AddedBy?.ToString(), ColumnName = "AddedBy" });
            if (auditList.Count() > 0)
            {
                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                {
                    HRMasterAuditTrailItems = auditList,
                    Type = "LevelMaster",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = queryEntity?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
