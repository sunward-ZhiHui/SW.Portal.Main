using Application.Command.LayoutPlanType;
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

namespace Application.Handlers.CommandHandler
{
    public class EditLevelMasterHandler : IRequestHandler<EditLevelMasterCommand, LevelMasterResponse>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        private readonly ILevelMasterQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditLevelMasterHandler(ILevelMasterCommandRepository customerRepository, ILevelMasterQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<LevelMasterResponse> Handle(EditLevelMasterCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.LevelId);
            var queryrEntity = RoleMapper.Mapper.Map<LevelMaster>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
                if (result != null)
                {
                    bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    if (result?.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = queryrEntity?.Name, ColumnName = "Name" });
                    }
                    if (result?.CompanyID != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyID?.ToString(), CurrentValue = queryrEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyName, CurrentValue = queryrEntity?.CompanyName, ColumnName = "SectionName" });
                    }
                    if (result?.DivisionID != queryrEntity?.DivisionID)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionID?.ToString(), CurrentValue = queryrEntity?.DivisionID?.ToString(), ColumnName = "DivisionID" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DivisionName, CurrentValue = queryrEntity?.DivisionName, ColumnName = "DivisionName" });
                    }
                    if (result?.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), CurrentValue = queryrEntity?.StatusCodeId.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = queryrEntity?.Name, PreValue = queryrEntity?.Name, ColumnName = "DisplayName" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "LevelMaster",
                            FormType = "Update",
                            HRMasterSetId = result?.LevelID,
                            AuditUserId = queryrEntity?.ModifiedByUserId,
                        };
                        await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                    }

                }
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new LevelMasterResponse
            {
                LevelID = queryrEntity.LevelId,
                AddedByUserID = queryrEntity.StatusCodeId,
                Name = queryrEntity.Name,
            };

            return response;
        }
    }
}
