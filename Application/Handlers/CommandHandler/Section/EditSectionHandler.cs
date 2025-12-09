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

namespace Application.Handlers.CommandHandler
{
    public class EditSectionHandler : IRequestHandler<EditSectionCommand, SectionResponse>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly ISectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;

        public EditSectionHandler(ISectionCommandRepository customerRepository, ISectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<SectionResponse> Handle(EditSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.SectionId);
            var queryrEntity = RoleMapper.Mapper.Map<Section>(request);

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
                    if (result?.SectionCode != queryrEntity.Code)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionCode, CurrentValue = queryrEntity?.Code, ColumnName = "Code" });
                    }
                    if (result?.SectionName != queryrEntity?.Name)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName, CurrentValue = queryrEntity?.Name, ColumnName = "Name" });
                    }
                    if (result?.Description != queryrEntity?.Description)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = queryrEntity?.Description, ColumnName = "Description" });
                    }
                    if (result?.DepartmentId != queryrEntity?.DepartmentId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentId?.ToString(), CurrentValue = queryrEntity?.DepartmentId?.ToString(), ColumnName = "DepartmentId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.DepartmentName, CurrentValue = queryrEntity?.DepartmentName, ColumnName = "DepartmentId" });
                    }
                    if (result?.ProfileCode != queryrEntity?.ProfileCode)
                    {

                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ProfileCode, CurrentValue = queryrEntity?.ProfileCode, ColumnName = "ProfileCode" });
                    }
                    if (result?.StatusCodeId != queryrEntity?.StatusCodeId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), CurrentValue = queryrEntity?.StatusCodeId.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = queryrEntity?.StatusCode, ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { CurrentValue = queryrEntity?.Name, PreValue = queryrEntity?.Name, ColumnName = "DisplayName" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "Section",
                            FormType = "Update",
                            HRMasterSetId = result?.SectionId,
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
            var response = new SectionResponse
            {
                SectionId = queryrEntity.SectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SectionName = queryrEntity.Name,
            };

            return response;
        }
    }
}
