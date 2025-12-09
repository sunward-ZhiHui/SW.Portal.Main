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

            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Name, ColumnName = "Name" });
            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.Name, CurrentValue = queryEntity?.Name, ColumnName = "DisplayName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Code, ColumnName = "Code" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Description, ColumnName = "Description" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.CompanyName, ColumnName = "CompanyName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.SectionId?.ToString(), ColumnName = "SectionId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.SectionName, ColumnName = "SectionName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.SubSectionId?.ToString(), ColumnName = "SubSectionId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.SubSectionName, ColumnName = "SubSectionName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.LevelId?.ToString(), ColumnName = "LevelId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.LevelName, ColumnName = "LevelName" });
            if (queryEntity?.HeadCount > 0)
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.HeadCount?.ToString(), ColumnName = "HeadCount" });
            }
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
                    Type = "Designation",
                    FormType = "Add",
                    HRMasterSetId = data,
                    AuditUserId = queryEntity?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
