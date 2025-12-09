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
            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Name, ColumnName = "Name" });
            auditList.Add(new HRMasterAuditTrail { PreValue = queryEntity?.Name, CurrentValue = queryEntity?.Name, ColumnName = "DisplayName" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Code, ColumnName = "Code" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.Description, ColumnName = "Description" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DepartmentId?.ToString(), ColumnName = "DepartmentId" });
            auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.DepartmentName, ColumnName = "DepartmentName" });
            if (!string.IsNullOrEmpty(queryEntity?.ProfileCode))
            {
                auditList.Add(new HRMasterAuditTrail { CurrentValue = queryEntity?.ProfileCode, ColumnName = "ProfileCode" });
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
                    Type = "Section",
                    FormType = "Add",
                    HRMasterSetId = plantData,
                    AuditUserId = queryEntity?.AddedByUserId,
                };
                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
            }
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
