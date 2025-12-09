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
    public class EditPlantHandler : IRequestHandler<EditPlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IPlantQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditPlantHandler(IPlantCommandRepository customerRepository, IPlantQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<PlantResponse> Handle(EditPlantCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.PlantID);
            var queryrEntity = RoleMapper.Mapper.Map<Plant>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            if (result != null)
            {
                bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                if (result.PlantCode != queryrEntity.PlantCode)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, CurrentValue = queryrEntity?.PlantCode, ColumnName = "PlantCode" });
                }
                if (result?.Description != queryrEntity?.Description)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = queryrEntity?.Description, ColumnName = "Description" });
                }
                if (result?.StatusCodeID != queryrEntity?.StatusCodeID)
                {
                    isUpdate = true;
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), CurrentValue = queryrEntity?.StatusCodeID?.ToString(), ColumnName = "StatusCodeID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                }
                if (isUpdate)
                {
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, CurrentValue = queryrEntity?.PlantCode, ColumnName = "DisplayName" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserID?.ToString(), ColumnName = "ModifiedByUserID" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                }
                if (auditList.Count() > 0)
                {
                    HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                    {
                        HRMasterAuditTrailItems = auditList,
                        Type = "Plant",
                        FormType = "Update",
                        HRMasterSetId = result?.PlantID,
                        AuditUserId = queryrEntity?.ModifiedByUserID,
                    };
                    await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                }
            }
            var response = new PlantResponse
            {
                PlantID = queryrEntity.PlantID,
                CompanyID = queryrEntity.CompanyID,
                PlantCode = queryrEntity.PlantCode,
                Description = queryrEntity.Description,
            };

            return response;
        }
    }
}
