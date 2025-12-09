using Application.Command.Departments;
using Application.Command.EmployeeOtherDutyInformations;
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
    public class EditEmployeeOtherDutyInformationHandler : IRequestHandler<EditEmployeeOtherDutyInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        private readonly IEmployeeOtherDutyInformationQueryRepository _queryRepository;
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public EditEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository customerRepository, IEmployeeOtherDutyInformationQueryRepository customerQueryRepository, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeOtherDutyInformation>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                var result = await _queryRepository.GetByIdAsync(queryrEntity.EmployeeOtherDutyInformationId);
                await _commandRepository.UpdateAsync(queryrEntity);

                if (result != null)
                {
                    bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                    if (result.DesignationId != queryrEntity.DesignationId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.DesignationId?.ToString(), CurrentValue = queryrEntity?.DesignationId?.ToString()?.ToString(), ColumnName = "DesignationId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.DesignationName?.ToString(), CurrentValue = queryrEntity?.DesignationName?.ToString(), ColumnName = "DesignationName" });
                    }
                    if (queryrEntity?.StartDate != result.StartDate)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StartDate != null ? result.StartDate.Value.ToString("dd-MMM-yyyy") : null, CurrentValue = queryrEntity?.StartDate != null ? queryrEntity.StartDate.Value.ToString("dd-MMM-yyyy") : null, ColumnName = "StartDate" });

                    }
                    if (queryrEntity?.EndDate != result?.EndDate)
                    {
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.EndDate != null ? result.EndDate.Value.ToString("dd-MMM-yyyy") : null, CurrentValue = queryrEntity?.EndDate != null ? queryrEntity.EndDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "EndDate" });
                    }
                    if (queryrEntity?.DutyTypeId != result?.DutyTypeId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.DutyTypeId?.ToString(), CurrentValue = queryrEntity?.DutyTypeId?.ToString()?.ToString(), ColumnName = "DutyTypeId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result.DutyType?.ToString(), CurrentValue = queryrEntity?.DutyTypeName?.ToString(), ColumnName = "DutyType" });
                    }
                    if (queryrEntity?.StatusCodeId != result?.StatusCodeId)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString()?.ToString(), CurrentValue = queryrEntity?.StatusCodeId?.ToString()?.ToString(), ColumnName = "StatusCodeId" });
                        isUpdate = true;
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode?.ToString(), CurrentValue = queryrEntity?.StatusCode?.ToString(), ColumnName = "StatusCode" });
                    }
                    if (isUpdate)
                    {
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = queryrEntity?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserId" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = queryrEntity?.ModifiedDate != null ? queryrEntity.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = queryrEntity?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                        auditList.Add(new HRMasterAuditTrail { PreValue = result?.EmployeeId?.ToString(), CurrentValue = queryrEntity?.EmployeeId?.ToString(), ColumnName = "EmployeeId" });
                    }
                    if (auditList.Count() > 0)
                    {
                        HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                        {
                            HRMasterAuditTrailItems = auditList,
                            Type = "EmployeeOtherDutyInformation",
                            FormType = "Update",
                            HRMasterSetId = result?.EmployeeOtherDutyInformationId,
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
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
