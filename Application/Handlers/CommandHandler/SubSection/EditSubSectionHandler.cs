using Application.Command.Departments;
using Application.Command.SubSections;
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
    public class EditSubSectionHandler : IRequestHandler<EditSubSectionCommand, SubSectionResponse>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        private readonly ISubSectionQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditSubSectionHandler(ISubSectionCommandRepository customerRepository, ISubSectionQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<SubSectionResponse> Handle(EditSubSectionCommand request, CancellationToken cancellationToken)
        {
            var result = await _queryRepository.GetByIdAsync(request.SubSectionId);
            var queryrEntity = RoleMapper.Mapper.Map<SubSection>(request);

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
                    if (result?.SubSectionCode != queryrEntity?.Code)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SubSectionCode, CurrentValue = queryrEntity?.Code, ColumnName = "Code" });
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
                    if (result?.SectionId != queryrEntity?.SectionId)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionId?.ToString(), CurrentValue = queryrEntity?.SectionId?.ToString(), ColumnName = "SectionId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.SectionName, CurrentValue = queryrEntity?.SectionName, ColumnName = "SectionName" });
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
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "SubSection",
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
            var response = new SubSectionResponse
            {
                SubSectionId = queryrEntity.SubSectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SubSectionName = queryrEntity.Name,
            };

            return response;
        }
    }
}
