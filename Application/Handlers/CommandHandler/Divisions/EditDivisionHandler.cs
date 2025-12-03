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
    public class EditDivisionHandler : IRequestHandler<EditDivisionCommand, DivisionResponse>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IDivisionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDivisionHandler(IDivisionCommandRepository customerRepository, IDivisionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DivisionResponse> Handle(EditDivisionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DivisionId);
            var queryrEntity = RoleMapper.Mapper.Map<Division>(request);

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
                    if (result.Code != queryrEntity.Code)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Code, CurrentValue = queryrEntity?.Code, ColumnName = "Code" });
                    }
                    if (result?.Name != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = queryrEntity?.Name, ColumnName = "Name" });
                    }
                    if (result?.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = queryrEntity?.Description, ColumnName = "Description" });
                    }
                    if (result?.CompanyId != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyId?.ToString(), CurrentValue = queryrEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.PlantCode, CurrentValue = queryrEntity?.CompanyName, ColumnName = "CompanyName" });
                    }
                    if (result?.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), CurrentValue = queryrEntity?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = queryrEntity?.Name, ColumnName = "DisplayName" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserID" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Division",
                            FormType = "Update",
                            HRMasterSetId = result?.DivisionID,
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
            var response = new DivisionResponse
            {
                DivisionID = queryrEntity.DivisionId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
