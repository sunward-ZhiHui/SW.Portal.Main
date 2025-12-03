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

namespace Application.Handlers.CommandHandler
{
    public class EditDesignationHandler : IRequestHandler<EditDesignationCommand, DesignationResponse>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditDesignationHandler(IDesignationCommandRepository customerRepository, IDesignationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<DesignationResponse> Handle(EditDesignationCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.DesignationId);
            var queryrEntity = RoleMapper.Mapper.Map<Designation>(request);

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
                    if (result?.Code != queryrEntity?.Code)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Code, CurrentValue = queryrEntity?.Code, ColumnName = "Code" });
                    }
                    if (result?.SubSectionName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionName, CurrentValue = queryrEntity?.Name, ColumnName = "Name" });
                    }
                    if (result?.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = queryrEntity?.Description, ColumnName = "Description" });
                    }
                    if (result?.CompanyID != queryrEntity?.CompanyId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyID?.ToString(), CurrentValue = queryrEntity?.CompanyId?.ToString(), ColumnName = "CompanyId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.CompanyName, CurrentValue = queryrEntity?.CompanyName, ColumnName = "CompanyName" });
                    }
                    if (result?.SectionID != queryrEntity?.SectionId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionID?.ToString(), CurrentValue = queryrEntity?.SectionId?.ToString(), ColumnName = "SectionId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName, CurrentValue = queryrEntity?.SectionName, ColumnName = "SectionName" });
                    }
                    if (result?.SubSectionID != queryrEntity?.SubSectionId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionID?.ToString(), CurrentValue = queryrEntity?.SubSectionId?.ToString(), ColumnName = "SubSectionId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionName, CurrentValue = queryrEntity?.SubSectionName, ColumnName = "SubSectionName" });
                    }
                    if (result?.LevelID != queryrEntity?.LevelId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.LevelID?.ToString(), CurrentValue = queryrEntity?.LevelId?.ToString(), ColumnName = "LevelId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.LevelName, CurrentValue = queryrEntity?.LevelName, ColumnName = "LevelName" });
                    }
                    if (result?.HeadCount != queryrEntity?.HeadCount)
                    {

                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.HeadCount?.ToString(), CurrentValue = queryrEntity?.HeadCount?.ToString(), ColumnName = "HeadCount" });
                    }
                    if (result?.StatusCodeID != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeID?.ToString(), CurrentValue = queryrEntity?.StatusCodeId.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = queryrEntity?.Name, CurrentValue = queryrEntity?.Name, ColumnName = "DisplayName" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserID?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Designation",
                            FormType = "Update",
                            HRMasterSetId = result?.DesignationID,
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
            var response = new DesignationResponse
            {
                DesignationID = queryrEntity.DesignationId,
                CompanyID = queryrEntity.CompanyId.Value,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
                Name = queryrEntity.Name,
            };

            return response;
        }
    }
}
